let gDP = '/data/';
let gGraphStrokeColor = '#AAAAAA';
let gGraphFontColor = '#EEEEEE';
let gGraphFillColor = '#AAAAAA';
let gGraphFontPixelSize = 14;
let gGraphFont = "Verdana";
let gGraphLineDash = [5, 5];
let gGraphPointRadius = 5;
let gGraphHeight = 300;
let gTruncateLength = 1;
let gDonutHeight = 300;
let gDonutPadding = 5;
let gDonutHoleSize = (gDonutHeight * .5) * .65;
let gDonutFontPixelSize = gDonutHoleSize * .7;
let gDonutFont = "Verdana";
let gBestOfCount = 3;
let gRatingColors = ["#f8696b", "#fbaa77", "#ffeb84", "#b1d580", "#63be7e"];
let gEpsRankedCount = 3;
let gPathName = window.location.pathname.replace(/\//g, '');
let gUrlParams = new URLSearchParams(window.location.search);
let gImdbId = gPathName != 'random' && gPathName != '' ? gPathName.split('-')[gPathName.split('-').length - 1] : '';
let gPosterImgHeight = 200;

function fdda(eps, avg) {
    let c = document.getElementById('episodeQuality').getContext('2d');
    $("#episodeQuality").attr('height', gDonutHeight);
    $("#episodeQuality").attr('width', gDonutHeight);

    c.height = gDonutHeight;
    c.width = gDonutHeight
    var ec = eps.length;

    var r = Array(11).fill(0);
    eps.map(x => {
        var rg = Math.floor(x.Rating);
        rg = rg <= 5 ? 5 : rg > 9 ? 9 : rg;
        r[rg]++;
    });

    var a = [];
    for (var i = 5; i < r.length; i++) {
        a.push(((r[i] / ec) * 2) * Math.PI);
    }

    var ba = 2 * Math.PI;
    var ea = 2 * Math.PI;

    for (var i = 0; i < a.length; i = i + 1) {
        ba = ea;
        ea = ea + a[i];

        c.beginPath();
        c.fillStyle = gRatingColors[i % gRatingColors.length];

        c.moveTo(gDonutHeight / 2, gDonutHeight / 2);
        c.arc(gDonutHeight / 2, gDonutHeight / 2, (gDonutHeight / 2) - 5, ba, ea);
        c.fill();
    }

    c.beginPath();
    c.fillStyle = $("body").css('background-color');
    c.moveTo(gDonutHeight / 2, gDonutHeight / 2);
    c.arc(gDonutHeight / 2, gDonutHeight / 2, gDonutHoleSize, 0, 2 * Math.PI);
    c.stroke();
    c.fill();
    c.fillStyle = gGraphFontColor;
    c.fontStyle = 'normal';
    c.fontWeight = 'bold';
    c.font = gDonutFontPixelSize + 'px ' + gDonutFont;
    c.textAlign = "center";
    c.textBaseline = "middle";
    c.fillText(avg, gDonutHeight / 2, gDonutHeight / 2);

}

function fddr(s, trt) {
    let c = document.getElementById('episodeRuntime').getContext('2d');
    $("#episodeRuntime").attr('height', gDonutHeight);
    $("#episodeRuntime").attr('width', gDonutHeight);

    c.height = gDonutHeight;
    c.width = gDonutHeight

    var a = [];
    s.map((x) => {
        a.push(((x.SeasonRunTimeMinutes / trt) * 2) * Math.PI);
    });

    var sc = fGc("ccffcc", "66ff66", a.length);

    var ba = 2 * Math.PI;
    var ea = 2 * Math.PI;

    for (var i = 0; i < a.length; i = i + 1) {
        ba = ea;
        ea = ea + a[i];

        c.beginPath();
        c.fillStyle = sc[i % sc.length];

        c.moveTo(gDonutHeight / 2, gDonutHeight / 2);
        c.arc(gDonutHeight / 2, gDonutHeight / 2, (gDonutHeight / 2) - 5, ba, ea);
        c.fill();
    }

    c.beginPath();
    c.fillStyle = $("body").css('background-color');
    c.moveTo(gDonutHeight / 2, gDonutHeight / 2);
    c.arc(gDonutHeight / 2, gDonutHeight / 2, gDonutHoleSize, 0, 2 * Math.PI);
    c.stroke();
    c.fill();
    c.fillStyle = gGraphFontColor;
    c.fontStyle = 'normal';
    c.fontWeight = 'bold';
    c.font = gDonutFontPixelSize / 2 + 'px ' + gDonutFont;
    c.textAlign = "center";
    c.textBaseline = "top";
    var rtl = fGd(trt).split('\n');
    var lh = gGraphFontPixelSize * 3;
    var tsy = (gDonutHeight / 2) - (lh * (rtl.length / 2));
    for (var i = 0; i < rtl.length; i++) {
        c.fillText(rtl[i], gDonutHeight / 2, tsy + (i * lh));
    }

}

function fPh(h) {
    var r = h.substr(0, 2);
    var g = h.substr(2, 2);
    var b = h.substr(4, 2);

    return [
        parseInt(r, 16),
        parseInt(g, 16),
        parseInt(b, 16)
    ]
}

function fp(n, w, z) {
    z = z || '0';
    n = n + '';
    return n.length >= w ? n : new Array(w - n.length + 1).join(z) + n;
}

function fGc(f, l, s) {
    var cs = fPh(f);
    var ce = fPh(l);

    var c = [];

    var sI = parseInt(s, 10);
    var sP = 100 / (sI + 1);

    var vc = [
        ce[0] - cs[0],
        ce[1] - cs[1],
        ce[2] - cs[2]
    ];

    for (var i = 0; i < sI; i++) {
        var cR = (vc[0] > 0)
            ? fp((Math.round(vc[0] / 100 * (sP * (i + 1)))).toString(16), 2)
            : fp((Math.round((cs[0] + (vc[0]) / 100 * (sP * (i + 1))))).toString(16), 2);

        var cG = (vc[1] > 0)
            ? fp((Math.round(vc[1] / 100 * (sP * (i + 1)))).toString(16), 2)
            : fp((Math.round((cs[1] + (vc[1]) / 100 * (sP * (i + 1))))).toString(16), 2);

        var cB = (vc[2] > 0)
            ? fp((Math.round(vc[2] / 100 * (sP * (i + 1)))).toString(16), 2)
            : fp((Math.round((cs[2] + (vc[2]) / 100 * (sP * (i + 1))))).toString(16), 2);

        c[i] = [
            '#',
            cR,
            cG,
            cB
        ].join('');
    }

    return c;
}

function fGd(m) {
    if (isNaN(m)) {
        return "";
    }
    var w = Math.floor(m / 10080);
    var t = "";
    if (w > 0) {
        m -= w * 10080;
        t += w + "w\n";
    }
    var d = Math.floor(m / 1440);
    if (d > 0) {
        m -= d * 1440;
        t += d + "d\n";
    }
    var h = Math.floor(m / 60);
    if (h > 0) {
        m -= h * 60;
        t += h + "h\n";
    }

    t += m + "m";

    return (t);
}

function fdg(s, ta, sc) {
    let c = document.getElementById('seasonGraph').getContext('2d');
    let cGw = $("div#graph").width();
    $("#seasonGraph").attr('height', gGraphHeight);
    $("#seasonGraph").attr('width', cGw);

    var cW = 1;
    s.map(x => {
        if (Math.abs(ta - x.SeasonRank) > cW) {
            cW = Math.abs(ta - x.SeasonRank);
        }
    });

    c.strokeStyle = gGraphStrokeColor;
    c.fillStyle = gGraphFillColor;
    c.fontStyle = 'normal';
    c.fontWeight = 'bold';
    c.font = gGraphFontPixelSize + 'px ' + gGraphFont;

    //draw top line
    c.beginPath();
    c.setLineDash([]);
    c.moveTo(gGraphPointRadius * 2, 1);
    c.lineTo(cGw - (gGraphPointRadius * 2), 1);
    c.stroke();

    //draw 0.5 dash
    c.beginPath();
    c.setLineDash(gGraphLineDash);
    c.moveTo(gGraphPointRadius * 2, gGraphHeight * .5);
    c.lineTo(cGw - (gGraphPointRadius * 2), gGraphHeight * .5);
    c.stroke();

    //draw top line
    c.beginPath();
    c.setLineDash([]);
    c.moveTo(gGraphPointRadius * 2, gGraphHeight - 1);
    c.lineTo(cGw - (gGraphPointRadius * 2), gGraphHeight - 1);
    c.stroke();

    var fl = true;
    var pm = gGraphPointRadius * 2;
    var cS = 1;

    s
        .sort(
            (a, b) =>
                a.SeasonNumber > b.SeasonNumber ? 1 :
                    a.SeasonNumber < b.SeasonNumber ? -1 :
                        0
        )
        .map(
            x => {
                var avg = x.SeasonRank;
                var diff = ta - avg;
                var d = (diff / (cW) * (gGraphHeight - gGraphPointRadius * 4) / 2);

                var nPx = pm;
                var nPy = (gGraphHeight / 2) + d;

                if (fl) {
                    c.beginPath();
                    c.moveTo(nPx, nPy);
                    fl = false;
                }
                else {
                    c.lineTo(nPx, nPy);
                    c.stroke();
                    c.beginPath();
                }

                c.beginPath();
                c.arc(nPx, nPy, gGraphPointRadius, 0, 2 * Math.PI);
                c.fill();

                c.textAlign = cS == 1 ? "left" : cS == s.length ? "right" : "center";
                c.fillStyle = gGraphFontColor;
                c.fillText(x.SeasonNumber.truncateTextOnly(), nPx, nPy + (d < 0 ? gGraphFontPixelSize * 2 : gGraphFontPixelSize * -2));
                c.fillStyle = gGraphFillColor;
                c.moveTo(nPx, nPy);
                pm += (cGw - (gGraphPointRadius * 4)) / (sc - 1);
                cS++;
            }
        );
}

String.prototype.truncateTextOnly = function () {
    return isNaN(this.substring(0, 1)) ? this.substring(0, gTruncateLength) : this;
};

function fLd() {
    if (
        gPathName &&
        gPathName != 'random' &&
        gPathName != ''
    ) {
        $.getJSON(gDP + gPathName + ".json", function (d) {
            const dS = [...new Set(d.Episodes.map(x => x.Season))];
            const dE = [...new Set(d.Episodes.map(x => x.Episode))];
            const dST = d.ShowTitle;
            const gs = d.Genres;
            document.title = 'What To Watch On TV -- Episode Ratings Visualizer -- ' + dST;

            var keywords = gs && gs.length > 0 ? gs.map((gfl) => { return gfl.GenreName }) : [];
            keywords.push(dST, "tv", "television", "imdb");

            document.getElementsByTagName('meta')["keywords"].content = keywords.join(",");
            document.getElementsByTagName('meta')["description"].content = dST + ": episode chart, watch time and rating graph, all episodes, and season ratings.";
            $('meta[property="og:image"]').attr('content', '/screenshots/' + gPathName + '.jpg');

            $("#SeriesTitle").html(dST);

            gs
                .filter((gfl) => {
                    return gfl.Rank > 0;
                })
                .map((gfl) => {
                    $("#GenreRanks").append("<a href='genres/" + gfl.GenreName + "'><span class='" + (gfl.Rank >= 5 ? "" : "rating" + (10 - gfl.Rank) + " ") + "mx-1 badge badge-pill badge-secondary'>#" + gfl.Rank + " in " + gfl.GenreName + "</span></a>");
                });

            $("th.episodeHeader").attr('rowspan', d.MaxEpisodes + 3);
            $("th.seasonHeader").attr('colspan', d.SeasonCount + 2);

            var hRow = "<tr>";
            hRow += "<th class='noBorder'></th>";
            hRow += "<th class='noBorder'></th>";
            dS.sort().map(x => {
                hRow += "<th class='seasonNumber'>" + (isNaN(x) ? x.truncateTextOnly() : x.padStart(2, '0')) + "</th>";
            });
            hRow += "</tr>";

            var bRow = "";
            dE.sort().map(x => {
                bRow += "<tr>";
                bRow += "<th class='episodeNumber'>" + x + "</th>";
                dS.map(xx => {
                    var ep = d.Episodes.filter(xxx => xxx.Season == xx && xxx.Episode == x);
                    if (ep.length > 0) {
                        bRow += "<td data-toggle='tooltip' data-placement='top' title='" + ep[0].EpisodeName.replace(/&/, "&amp;").replace(/"/g, "&quot;").replace(/'/g, "&#39;") + "' class='rating" + Math.floor(ep[0].Rating) + "'>" + ep[0].Rating.toFixed(1) + "</td>";
                    }
                    else {
                        bRow += "<td class='noBorder'></td>";
                    }
                })
                bRow += "</tr>";
            });

            var aRow = "<tr>";
            aRow += "<td class='noBorder'></td>";
            aRow += "<th>Avg</th>";
            d.Seasons
                .sort(
                    (a, b) =>
                        a.SeasonNumber > b.SeasonNumber ? 1 :
                            a.SeasonNumber < b.SeasonNumber ? -1 :
                                0
                )
                .map(
                    season => {
                        aRow += "<td>" + season.SeasonRank.toFixed(2) + "</td>";
                    }
                );
            aRow += "</tr>";

            $("table#ratings thead tr:last").after(hRow);
            $("table#ratings tbody tr:last").after(bRow);
            $("table#ratings tfoot").html(aRow);

            d.Episodes.sort((a, b) => b.Rating - a.Rating);
            var eRR = "<table id='episodeList' class='table table-dark table-sm'>";
            eRR += "<thead>";
            eRR += "<tr>";
            eRR += "<th>Rank</th>";
            eRR += "<th>Ep</th>";
            eRR += "<th>Episode Name</th>";
            eRR += "<th>Rating</th>";
            eRR += "</tr>";
            eRR += "<tbody>";
            for (var i = 0; i < d.Episodes.length; i++) {
                var cols = "<td>" + (i + 1) + "</td>";
                cols += "<td>S" + d.Episodes[i].Season.truncateTextOnly() + "E" + d.Episodes[i].Episode + "</td>";
                cols += "<td>" + d.Episodes[i].EpisodeName + "</td>";
                cols += "<td>" + d.Episodes[i].Rating.toFixed(1) + "</td>";
                if (i < gEpsRankedCount || i > d.Episodes.length - (gEpsRankedCount + 1)) {
                    eRR += "<tr class='rating" + Math.floor(d.Episodes[i].Rating) + " border-top'>";
                    eRR += cols;
                    eRR += "</tr>";
                }
                else if (i == gEpsRankedCount && !gUrlParams.has('print')) {
                    eRR += "<tr class='more border-top'>";
                    eRR += "<td colspan='4'><button id='showAllEps' type='button' class='btn btn-link'>Show All Episodes</button></td>";
                    eRR += "</tr>";
                    eRR += "<tr class='hidden rating" + Math.floor(d.Episodes[i].Rating) + " border-top'>";
                    eRR += cols;
                    eRR += "</tr>";
                }
                else {
                    eRR += "<tr class='hidden rating" + Math.floor(d.Episodes[i].Rating) + " border-top'>";
                    eRR += cols;
                    eRR += "</tr>";
                }
            }
            eRR += "</tbody>";
            $("div#epsRanked div").append(eRR);

            fdg(d.Seasons, d.TotalAverage, d.SeasonCount);
            fdda(d.Episodes, d.TotalAverage.toFixed(2));
            fddr(d.Seasons, d.RunTimeMinutes);
        });
    }
}

function fSoL(m) {
    var l = $("#searchMenu");
    var lI = l.children("li").get();
    lI.sort(function (a, b) {
        switch (m) {
            case 'alpha':
                return $(a).first("a").text().toUpperCase().localeCompare($(b).first("a").text().toUpperCase());
            case 'vote':
                return $(a).data('id') - $(b).data('id');
        }
    });
    $.each(lI, function (idx, itm) { l.append(itm); });
}

function fSL(sl) {
    if (gPathName == '' || gPathName == 'random') {
        if (localStorage.getItem('ratingFilter')) {
            sl = sl.filter((x) => {
                return x.Rating >= localStorage.getItem('ratingFilter');
            });
        }
        window.location.href = '/' + sl[Math.floor(Math.random() * sl.length)].CleanTitle;
    }
    else {
        var nE = "";
        var sC = 0;
        sl.map(x => {
            nE += "<li data-rating='" + x.Rating + "' data-id='" + (sC++) + "'><a href='/" + x.CleanTitle + "'>" + x.Title + "</a></li>";
        });
        var te = $("ul#searchMenu li:last").length > 0 ? $("ul#searchMenu li:last") : $("input#searchInput");
        $(te).after(nE);
    }
}

function fFbR(r) {
    $("#searchMenu li").filter(function () {
        $(this).toggle($(this).data('rating') >= r);
    });
    $("#searchMenu").find("button.starRating").each(function (index, value) {
        $(value).html($(value).data("rating") > r ? "☆" : "★");
    });
    $(".tooltip").tooltip("hide");
}

function fSetLS(key, value, ttl) {
    const now = new Date();
    const item = {
        value: value,
        expiry: now.getTime() + ttl
    }
    localStorage.setItem(key, JSON.stringify(item));
}

function fGetLS(key) {
    const itemStr = localStorage.getItem(key);
    if (!itemStr) {
        return null;
    }

    const item = JSON.parse(itemStr)
    if (!item.expiry) {
        return null;
    }

    const now = new Date();

    if (now.getTime() > item.expiry) {
        localStorage.removeItem(key);
        return null;
    }
    return item.value;
}

$(document).ready(function () {
    fLd();
    $('[data-toggle="tooltip"]').tooltip()
    $("#searchInput").on("keyup", function () {
        var value = $(this).val().toLowerCase();
        $(".dropdown-menu li").filter(function () {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });

    var sl = fGetLS('showList');
    if (!sl) {
        $.getJSON('/showList.json', function (x) {
            fSetLS('showList', JSON.stringify(x), 86400000);
            fSL(x);
        });
    }
    else {
        fSL(JSON.parse(sl));
    }

    $("button#btnSortAlpha").click(function (e) {
        fSoL("alpha");
        localStorage.setItem('sortOrder', 'alpha');
        $(".tooltip").tooltip("hide");
        e.stopPropagation();
    });
    $("button#btnSortVotes").click(function (e) {
        fSoL("vote");
        localStorage.setItem('sortOrder', 'vote');
        $(".tooltip").tooltip("hide");
        e.stopPropagation();
    });
    if (localStorage.getItem('sortOrder')) {
        fSoL(localStorage.getItem('sortOrder'));
    }
    if (localStorage.getItem('ratingFilter')) {
        fFbR(localStorage.getItem('ratingFilter'));
    }
    $("button.starRating").click(function (e) {
        var r = $(this).data('rating');
        localStorage.setItem('ratingFilter', r);
        fFbR(r);
        e.stopPropagation();
    });
    $("div#searchDropdown").on("shown.bs.dropdown", function () {
        $("input#searchInput").focus();
    });
    $(document).on('click', 'button#showAllEps', function () {
        $("table#episodeList tbody tr").removeClass('hidden');
        $("table#episodeList tbody tr.more").hide();
    });
    var cw = fGetLS('cookieWarning');
    if (!cw) {
        $("#cookieNotice").removeClass("d-none");
    }
    $("#acceptCookieButton").on('click', function () {
        fSetLS('cookieWarning', true, 2592000000);
        $("#cookieNotice").addClass("d-none");
    });

    $("#SeriesDetails").on('show.bs.collapse', function () {
        $.ajax({
            url: "https://www.omdbapi.com/?i=" + gImdbId + "&apikey=da6a49dd",
            context: document.body
        })
            .done(function (data) {
                $("#SeriesPlot").html(data.Plot);
                $("#SeriesActors").html("<span class='detailsProperty'>Actor(s)</span>: " + data.Actors);
                $("#SeriesAwards").html("<span class='detailsProperty'>Award(s)</span>: " + data.Awards);
            });
        $("#showSeriesDetails").html('Hide Series Description');
        $("img#MoviePoster").attr('src', "https://img.omdbapi.com/?i=" + gImdbId + "&h=" + gPosterImgHeight + "&apikey=da6a49dd");
    })
    $("#SeriesDetails").on('hide.bs.collapse', function () {
        $("#showSeriesDetails").html('Show Series Description');
    });

    $("a#viewOnImdb").attr('href','https://www.imdb.com/title/' + gImdbId);

    if (gUrlParams.has('print')){
        $("#searchRow").children().hide();
        $("#cookieNotice").hide();
        $("#beg").hide();
        $("#showSeriesDetails").hide();
    }
});
function hexToBase64(str) {
    return btoa(String.fromCharCode.apply(null, str.replace(/\r|\n/g, "").replace(/([\da-fA-F]{2}) ?/g, "0x$1 ").replace(/ +$/, "").split(" ")));
}

$.fn.extend({
    toggleHtml: function (a, b) {
        return this.html(this.html() == b ? a : b);
    }
});