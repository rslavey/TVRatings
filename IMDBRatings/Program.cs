using IMDBRatings.Properties;
using Newtonsoft.Json;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IMDBRatings
{
    class Program
    {
        private class ShowTitle
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public string CleanTitle { get; set; }
            public List<Genre> Genres = new List<Genre>();
            public double Rating { get; set; }
        }
        private class Rating
        {
            public string Id { get; set; }
            public double RatingScore { get; set; }
            public int Votes { get; set; }
        }
        private class TitleEpisode
        {
            public string Id { get; set; }
            public string ParentId { get; set; }
            public string Season { get; set; }
            public string Episode { get; set; }
            public string EpisodeName { get; set; }
            public double Rating { get; set; }
            public int RunTimeMinutes { get; set; }
            public int Votes { get; set; }
        }
        private class Genre
        {
            public string GenreName { get; set; }
            public int Rank { get; set; }
        }
        private class Show
        {
            public string Id { get; set; }
            public string ShowTitle { get; set; }
            public string CleanTitle { get; set; }
            public string ShowType { get; set; }
            public string StartYear { get; set; }
            public string EndYear { get; set; }
            public List<Genre> Genres = new List<Genre>();
            public int RunTimeMinutes { get; set; }
            public double TotalAverage { get; set; }
            public double MinRating { get; set; }
            public double MaxRating { get; set; }
            public int MaxEpisodes { get; set; }
            public int SeasonCount { get; set; }
            public int MinEpisode { get; set; }
            public int MinSeason { get; set; }
            public List<TitleEpisode> Episodes = new List<TitleEpisode>();
            public List<Season> Seasons = new List<Season>();
        }
        private class Season
        {
            public string SeasonNumber { get; set; }
            public double SeasonRank { get; set; }
            public int SeasonRunTimeMinutes { get; set; }
        }

        private class GenreChartData
        {
            public string Genre { get; set; }
            public List<GenreChartDataPoint> DataPoints = new List<GenreChartDataPoint>();
        }

        private class GenreChartDataPoint
        {
            public double Rating { get; set; }
            public int RatingCount { get; set; }
        }

        static void Main(string[] args)
        {
            var dataSets = $"{Settings.Default.FilePath}\\datasets";
            var allRatings = new List<Rating>();
            var allSeries = new List<Show>();
            var genreList = new List<string>();
            var showList = new List<ShowTitle>();

            if (Settings.Default.IsDebug)
            {
                foreach (var f in Directory.GetFiles($"{Settings.Default.FilePath}\\json"))
                {
                    var t = File.ReadAllText(f);
                    var o = JsonConvert.DeserializeObject<Show>(t);
                    allSeries.Add(o);
                }
                genreList = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText($"{Settings.Default.FilePath}\\genreList.json"));
            }
            else
            {
                DownloadIMDBFiles();
                Log($"Transforming ratings file");
                foreach (string line in File.ReadLines($@"{dataSets}\title.ratings.tsv", Encoding.UTF8))
                {
                    var l = line.Split('\t');
                    if (l.Length != 3)
                    {
                        continue;
                    }

                    if (double.TryParse(l[1], out double rating) && rating > 0)
                    {
                        allRatings.Add(new Rating
                        {
                            Id = l[0],
                            RatingScore = rating,
                            Votes = int.TryParse(l[2], out int votes) ? votes : 0
                        });
                    }
                }
                var ratingsDictionary = allRatings.Where(x => x.RatingScore > 0).ToDictionary(x => x.Id, x => x.RatingScore);

                Log($"Transforming titles file");
                var allTv = new List<Show>();
                var allMovies = new List<Show>();
                string prevId = "";
                foreach (string line in File.ReadLines($@"{dataSets}\title.basics.tsv", Encoding.UTF8))
                {
                    var l = line.Split('\t');
                    if (l.Length != 9)
                    {
                        continue;
                    }
                    if (l[0] == prevId)
                    {
                        continue;
                    }
                    prevId = l[0];
                    if (l[1] == "tvSeries" || l[1] == "tvEpisode" || l[1] == "tvMiniSeries")
                    {
                        var genres = new List<Genre>();
                        foreach (var g in l[8].Split(',').Length > 0 ? l[8].Split(',').Where(x => x != @"\N") : null)
                        {
                            genres.Add(new Genre
                            {
                                GenreName = g,
                                Rank = 0
                            });
                        }
                        allTv.Add(new Show()
                        {
                            Id = l[0],
                            ShowTitle = l[2],
                            ShowType = l[1],
                            StartYear = l[5],
                            EndYear = l[6],
                            RunTimeMinutes = int.TryParse(l[7], out int runTime) ? runTime : 0,
                            Genres = genres
                        });
                    }
                    if (l[1] == "movie" || l[1] == "videoGame" || l[1] == "tvSeries" || l[1] == "tvMiniSeries")
                    {
                        var s = new Show()
                        {
                            Id = l[0],
                            ShowTitle = l[2],
                            ShowType = l[1]
                        };
                        if (ratingsDictionary.TryGetValue(l[0], out double rating))
                        {
                            s.TotalAverage = rating;
                        }
                        allMovies.Add(s);
                    }
                }
                File.WriteAllText($@"{Settings.Default.FilePath}\movies.json", JsonConvert.SerializeObject(allMovies));

                genreList = allTv.SelectMany(x => x.Genres.Select(xx => xx.GenreName)).Distinct().ToList();
                if (genreList.Count() == 0)
                {
                    //Sometimes IMDB files are bad and missing the genre
                    Log($"Genres missing from titles file. Skipping this download.");
                    return;
                }

                Log($"Writing genre list file");
                WriteGenreListToJsonFiles(genreList);

                var allEps = new List<TitleEpisode>();
                var allTvSeries = new HashSet<string>(allTv.Where(x => x.ShowType == "tvSeries" || x.ShowType == "tvMiniSeries").Select(x => x.Id).Distinct());
                var allTvEpisodes = allTv.Where(x => x.ShowType == "tvEpisode").ToDictionary(x => x.Id, x => x.ShowTitle);
                var allTvGenres = allTv.Where(x => x.ShowType == "tvSeries" || x.ShowType == "tvMiniSeries").ToDictionary(x => x.Id, x => x.Genres);
                var topRatedSeries = new HashSet<string>(allRatings.Where(x => allTvSeries.Contains(x.Id)).OrderByDescending(x => x.Votes).Select(x => x.Id).Distinct());
                var runTimeMinutesDictionary = allTv.ToDictionary(x => x.Id, x => x.RunTimeMinutes);

                var votesDictionary = allRatings.Where(x => x.Votes > 10).ToDictionary(x => x.Id, x => x.Votes);

                Log($"Transforming episodes file");
                foreach (string line in File.ReadLines($@"{dataSets}\title.episode.tsv", Encoding.UTF8))
                {
                    var l = line.Split('\t');
                    if (l.Length != 4)
                    {
                        continue;
                    }
                    if (topRatedSeries.Contains(l[1]))
                    {
                        var newEp = new TitleEpisode
                        {
                            Id = l[0],
                            Season = l[2],
                            Episode = l[3],
                            ParentId = l[1],
                            EpisodeName = allTvEpisodes.TryGetValue(l[0], out string showTitle) ? showTitle : string.Empty,
                        };
                        if (ratingsDictionary.TryGetValue(l[0], out double rating))
                        {
                            newEp.Rating = rating;
                        }
                        if (runTimeMinutesDictionary.TryGetValue(l[0], out int runTimeMinutes))
                        {
                            newEp.RunTimeMinutes = runTimeMinutes;
                        }
                        if (votesDictionary.TryGetValue(l[0], out int votes))
                        {
                            newEp.Votes = votes;
                        }
                        allEps.Add(newEp);
                    }
                }


                Dictionary<string, string> allTvTitles = new Dictionary<string, string>();
                allTvTitles = allTv.ToDictionary(
                    x => x.Id,
                    x => $"{x.ShowTitle} ({x.StartYear} - {(x.EndYear == @"\N" ? string.Empty : x.EndYear)})"
                    );

                var allParentIds = allEps.GroupBy(x => x.ParentId).Where(x => x.Sum(xx => xx.Votes) > 500 && x.Sum(xx => xx.Rating) > 0).OrderByDescending(x => x.Sum(xx => xx.Votes)).Select(x => x.Key).ToHashSet();
                var showCount = 0;
                var showTotalCount = allParentIds.Count();
                var seasonPads = allEps.GroupBy(x => x.ParentId).ToDictionary(x => x.Key, x => x.Max(xx => xx.Season.Length));
                var episodePads = allEps.GroupBy(x => x.ParentId).ToDictionary(x => x.Key, x => x.Max(xx => xx.Episode.Length));
                foreach (var parentId in allParentIds)
                {
                    Log($"Generating show objects {++showCount} of {showTotalCount}");
                    string showTitle = allTvTitles.TryGetValue(parentId, out showTitle) ? showTitle : string.Empty;
                    List<Genre> genres = allTvGenres.TryGetValue(parentId, out genres) ? genres : new List<Genre>();
                    var cleanTitle = CleanTitle(showTitle, parentId);
                    var ser = new Show()
                    {
                        Id = parentId,
                        ShowTitle = showTitle,
                        CleanTitle = cleanTitle,
                        Genres = genres,
                    };

                    var unknownEpisodeCount = 0;
                    var seasonPad = seasonPads[parentId];
                    var episodePad = episodePads[parentId];
                    foreach (var ep in allEps.Where(x => x.ParentId == ser.Id && x.Rating > 0))
                    {
                        ser.Episodes.Add(new TitleEpisode
                        {
                            Id = ep.Id,
                            Season = ep.Season == @"\N" ? "Unknown" : ep.Season.PadLeft(seasonPad, '0'),
                            Episode = ep.Episode == @"\N" ? $"{unknownEpisodeCount++}".PadLeft(episodePad, '0') : ep.Episode.PadLeft(episodePad, '0'),
                            EpisodeName = ep.EpisodeName,
                            ParentId = ep.ParentId,
                            Rating = ep.Rating,
                            Votes = ep.Votes,
                            RunTimeMinutes = ep.RunTimeMinutes
                        });
                    }


                    ser.TotalAverage = ser.Episodes.Average(x => x.Rating);
                    ser.MinRating = ser.Episodes.Min(x => x.Rating);
                    ser.MaxRating = ser.Episodes.Max(x => x.Rating);
                    ser.MaxEpisodes = ser.Episodes.Select(x => x.Episode).Distinct().Count();
                    ser.SeasonCount = ser.Episodes.Select(x => x.Season).Distinct().Count();
                    ser.RunTimeMinutes = ser.Episodes.Sum(x => x.RunTimeMinutes);
                    foreach (var seasons in ser.Episodes.GroupBy(x => x.Season))
                    {
                        var avg = seasons.Average(x => x.Rating);
                        var runTimeMinutes = seasons.Sum(x => x.RunTimeMinutes);
                        ser.Seasons.Add(new Season
                        {
                            SeasonNumber = seasons.Key,
                            SeasonRank = avg,
                            SeasonRunTimeMinutes = runTimeMinutes
                        });
                    }

                    allSeries.Add(ser);
                }
            }

            foreach(var ser in allSeries.OrderByDescending(x => x.Episodes.Sum(xx => xx.Votes)))
            {
                showList.Add(new ShowTitle
                {
                    Id = ser.Id,
                    Title = ser.ShowTitle,
                    CleanTitle = ser.CleanTitle,
                    Rating = ser.TotalAverage,
                    Genres = ser.Genres
                });
            }

            Log($"Generating Sitemap");
            GenerateSitemap(showList);

            Log($"Updating Genre Rankings");
            RankGenres(allSeries);

            Log($"Create Genre Chart Datasets");
            GenerateGenreDataSet(allSeries, genreList);

            Log($"Writing show JSON files");
            WriteEpsToJsonFiles(allSeries);

            Log($"Writing image files");
            WriteImageFilesAsync(showList).Wait();

            Log($"Writing summary JSON file");
            WriteShowListToJsonFiles(showList);

            Log($"Creating Zip file of JSON files");
            AddJsonFilesToZip();

            Log($"Creating Zip file of screenshots");
            AddImagesToZip();

            Log($"Uploading Zip file of JSON files");
            UploadJSONZipFile();

            Log($"Uploading Zip file of screenshots");
            UploadScreenshotsZipFile();

            Log($"Uploading supporting files");
            UploadSupportingFiles();
        }

        private static void GenerateSitemap(List<ShowTitle> showList)
        {
            var sbSitemap = new StringBuilder();
            sbSitemap.AppendLine($"<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sbSitemap.AppendLine($"<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\"> ");
            sbSitemap.AppendLine($"	<url>");
            sbSitemap.AppendLine($"		<loc>{Settings.Default.TargetWebsite}</loc>");
            sbSitemap.AppendLine($"		<lastmod>2020-04-14</lastmod>");
            sbSitemap.AppendLine($"		<priority>1.0</priority>");
            sbSitemap.AppendLine($"	</url>");
            sbSitemap.AppendLine($"	<url>");
            sbSitemap.AppendLine($"		<loc>{Settings.Default.TargetWebsite}random</loc>");
            sbSitemap.AppendLine($"		<lastmod>2020-04-14</lastmod>");
            sbSitemap.AppendLine($"		<priority>1.0</priority>");
            sbSitemap.AppendLine($"	</url>");
            sbSitemap.AppendLine($"	<url>");
            sbSitemap.AppendLine($"		<loc>{Settings.Default.TargetWebsite}genres/genre-charts.html</loc>");
            sbSitemap.AppendLine($"		<lastmod>2020-07-06</lastmod>");
            sbSitemap.AppendLine($"		<priority>1.0</priority>");
            sbSitemap.AppendLine($"	</url>");

            foreach (var ser in showList)
            {
                sbSitemap.AppendLine($"	<url>");
                sbSitemap.AppendLine($"		<loc>{Settings.Default.TargetWebsite}{ser.CleanTitle}</loc>");
                sbSitemap.AppendLine($"		<lastmod>{DateTime.Now:yyyy-MM-dd}</lastmod>");
                sbSitemap.AppendLine($"		<changefreq>{Settings.Default.SitemapChangeFrequency}</changefreq>");
                sbSitemap.AppendLine($"	</url>");
            }

            sbSitemap.AppendLine($"</urlset>");

            Log($"Writing sitemap");
            File.WriteAllText($"{Settings.Default.FilePath}\\sitemap.xml", sbSitemap.ToString());
        }

        private static void GenerateGenreDataSet(List<Show> allSeries, List<string> genreList)
        {
            var gcdl = new List<GenreChartData>();
            foreach (var g in genreList)
            {
                var topShows = allSeries.Where(x => x.Genres.Select(xx => xx.GenreName).Contains(g) &&
                        x.Episodes.Min(xx => xx.Votes) > Settings.Default.TopShowsMinVotesRequired &&
                        x.Episodes.Count() > Settings.Default.TopShowsMinEpisodeCount);

                if (topShows.Count() <= Settings.Default.TopShowsMinCount)
                {
                    continue;
                }
                var gcd = new GenreChartData();
                gcd.Genre = g;
                
                var rs = topShows.Where(x => x.Genres.Select(xx => xx.GenreName).Contains(g)).SelectMany(x => x.Episodes.Select(xx => xx.Rating)).GroupBy(x => x);

                foreach (var rse in rs.OrderBy(x => x.Key))
                {
                    gcd.DataPoints.Add(new GenreChartDataPoint {
                        Rating = rse.Key,
                        RatingCount = rse.Count()
                    });
                }

                gcdl.Add(gcd);
            }
            File.WriteAllText($"{Settings.Default.FilePath}\\genreChartData.json", JsonConvert.SerializeObject(gcdl));
        }

        private static void UploadSupportingFiles()
        {
            WebClient client = new WebClient();
            client.Credentials = new NetworkCredential(Settings.Default.FtpUsername, Settings.Default.FtpPassword);
            client.UploadFile($"{Settings.Default.FtpAddress}//showList.json", $"{Settings.Default.FilePath}\\showList.json");
            client.UploadFile($"{Settings.Default.FtpAddress}//sitemap.xml", $"{Settings.Default.FilePath}\\sitemap.xml");
            client.UploadFile($"{Settings.Default.FtpAddress}//genreChartData.json", $"{Settings.Default.FilePath}\\genreChartData.json");
        }

        private static void RankGenres(List<Show> shows)
        {
            var genreShows = new Dictionary<string, List<Show>>();
            var genreList = shows.SelectMany(x => x.Genres.Select(xx => xx.GenreName));
            foreach(var g in genreList.Distinct())
            {
                var topVoted = shows.Where(x => x.Genres.Select(xx => xx.GenreName).Contains(g) &&
                    x.Episodes.Min(xx => xx.Votes) > Settings.Default.TopShowsMinVotesRequired &&
                    x.Episodes.Count() > Settings.Default.TopShowsMinEpisodeCount);
                genreShows.Add(g, topVoted.OrderByDescending(x => x.TotalAverage).ToList());
            }
            foreach(var ser in shows)
            {
                foreach (var genre in ser.Genres)
                {
                    var compareShows = shows.Where(x => x.Genres.Select(xx => xx.GenreName).Contains(genre.GenreName)).OrderByDescending(x => x.TotalAverage);
                    genre.Rank = genreShows.FirstOrDefault(x => x.Key == genre.GenreName).Value.FirstOrDefault(x => x == ser) == null ? 0 : genreShows.FirstOrDefault(x => x.Key == genre.GenreName).Value.IndexOf(ser);
                }

            }
        }

        private static void WriteGenreListToJsonFiles(IEnumerable<string> genreList)
        {
            if (genreList.Count() > 0)
            {
                var json = JsonConvert.SerializeObject(genreList);
                File.WriteAllText($"{Settings.Default.FilePath}\\genreList.json", json);
            }
        }

        private static void AddJsonFilesToZip()
        {
            var newJsonZipFile = $"{Settings.Default.FilePath}\\json.zip";
            if (File.Exists(newJsonZipFile))
            {
                File.Delete(newJsonZipFile);
            }
            using (ZipArchive archive = ZipFile.Open(newJsonZipFile, ZipArchiveMode.Create))
            {
                foreach (var f in Directory.GetFiles($"{Settings.Default.FilePath}\\json").Where(x => x.EndsWith(".json")))
                {
                    archive.CreateEntryFromFile(f, Path.GetFileName(f));
                }
            }
        }

        private static void AddImagesToZip()
        {
            var newZipFile = $"{Settings.Default.FilePath}\\screenshots.zip";
            if (File.Exists(newZipFile))
            {
                File.Delete(newZipFile);
            }
            using (ZipArchive archive = ZipFile.Open(newZipFile, ZipArchiveMode.Create))
            {
                foreach (var f in Directory.GetFiles($"{Settings.Default.FilePath}\\images").Where(x => x.EndsWith(".jpg")))
                {
                    archive.CreateEntryFromFile(f, Path.GetFileName(f));
                }
            }
        }

        private static void UploadJSONZipFile()
        {
            WebClient client = new WebClient();
            client.Credentials = new NetworkCredential(Settings.Default.FtpUsername, Settings.Default.FtpPassword);
            client.UploadFile($"{Settings.Default.FtpAddress}//json.zip", $"{Settings.Default.FilePath}\\json.zip");
        }

        private static void UploadScreenshotsZipFile()
        {
            WebClient client = new WebClient();
            client.Credentials = new NetworkCredential(Settings.Default.FtpUsername, Settings.Default.FtpPassword);
            client.UploadFile($"{Settings.Default.FtpAddress}//screenshots.zip", $"{Settings.Default.FilePath}\\screenshots.zip");
        }

        private static void WriteShowListToJsonFiles(List<ShowTitle> showList)
        {
            var showListJson = JsonConvert.SerializeObject(showList);
            File.WriteAllText($"{Settings.Default.FilePath}\\showList.json", showListJson);
        }

        private static void WriteEpsToJsonFiles(List<Show> allSeries)
        {
            var showCount = 0;
            var showTotalCount = allSeries.Count();
            foreach (var ser in allSeries)
            {
                Log($"Writing JSON files {++showCount} of {showTotalCount}");
                var json = JsonConvert.SerializeObject(ser);
                File.WriteAllText($"{Settings.Default.FilePath}\\json\\{ser.CleanTitle}.json", json);
            }
        }

        private static async Task WriteImageFilesAsync(List<ShowTitle> showList)
        {
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            Browser browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                DefaultViewport = null
            });

            var showCount = 0;
            var showTotalCount = showList.Count();
            
            foreach (var ser in showList.Take(Settings.Default.ScreenshotsToGenerate))
            {
                Log($"Writing image files {++showCount} of {showTotalCount}");
                Page page = await browser.NewPageAsync();
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = Settings.Default.ViewportWidthForScreenshot
                });
                await page.GoToAsync($"{Settings.Default.TargetWebsite}{ser.CleanTitle}?print");
                await page.GetContentAsync();
                await page.ScreenshotAsync($"{Settings.Default.FilePath}\\images\\{ser.CleanTitle}.jpg", options: new ScreenshotOptions{ FullPage = true  });
            }

        }

        private static string CleanTitle(string showTitle, string parentId)
        {
            var newTitle = Regex.Replace(showTitle, @"[^A-Za-z0-9_~]+", "-");
            return $"{((newTitle[newTitle.Length - 1] == '-' ? newTitle.Substring(0, newTitle.Length - 1) : newTitle).ToLowerInvariant())}-{parentId}";
        }

        static void DownloadIMDBFiles()
        {
            var oldestFile = new DirectoryInfo($"{Settings.Default.FilePath}\\datasets\\").GetFileSystemInfos().Where(x => x.Extension == ".gz").OrderBy(x => x.LastWriteTime).FirstOrDefault()?.LastWriteTime;
            if (oldestFile.HasValue && oldestFile.Value.Date == DateTime.Now.Date)
            {
                return;
            }
            using (var c = new WebClient())
            {
                foreach (var f in Settings.Default.IMDBFileNames.Split(','))
                {
                    Log($"Downloading {Settings.Default.IMDBUrlPath}{f}");
                    c.DownloadFile($"{Settings.Default.IMDBUrlPath}{f}", $"{Settings.Default.FilePath}\\datasets\\{f}");
                }
            }

            foreach (var f in Directory.GetFiles($"{Settings.Default.FilePath}\\datasets").Where(x => x.EndsWith(".gz")))
            {
                Log($"Decompressing {f}");
                Decompress(new FileInfo(f));
            }
        }

        private static void Log(string message, int logLevel = 0)
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}: {message}");
        }

        static void Decompress(FileInfo fileToDecompress)
        {
            using (FileStream originalFileStream = fileToDecompress.OpenRead())
            {
                string currentFileName = fileToDecompress.FullName;
                string newFileName = currentFileName.Remove(currentFileName.Length - fileToDecompress.Extension.Length);

                using (FileStream decompressedFileStream = File.Create(newFileName))
                {
                    using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(decompressedFileStream);
                        Console.WriteLine("Decompressed: {0}", fileToDecompress.Name);
                    }
                }
            }
        }

    }
}
