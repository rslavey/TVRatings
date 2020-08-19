let gPathName = window.location.pathname.replace('/genres/', '').replace(/\//g, '');
let chartHeight = 100;
let chartWidth = 200;
let gRatingColors = ["#f8696b", "#fbaa77", "#ffeb84", "#b1d580", "#63be7e", "#63be7e"];
let gLineWidth = 2;

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

function fSoL(a, b) {
    var aRank = a.Genres.filter((s) => {
        return s.GenreName == gPathName;
    })[0].Rank;
    var bRank = b.Genres.filter((s) => {
        return s.GenreName == gPathName;
    })[0].Rank;

    return aRank > bRank ? 1 :
        aRank < bRank ? -1 : 0
}

function fSL(cd) {

    cd.sort((a, b) => b.Genre > a.Genre ? -1 : a.Genre > b.Genre ? 1 : 0);
    cd.filter((gd) => gd.DataPoints.length > 0).map((gd) => {
        var g = gd.Genre;
        var dps = gd.DataPoints;
        console.log(g);
        console.log(dps);
        var maxDp = 0;
        console.log(maxDp);
        var cvs = document.createElement('canvas');
        var c = cvs.getContext('2d');
        cvs.id = "Canvas_" + g;
        cvs.width = chartWidth;
        cvs.height = chartHeight;
        var d = document.createElement('div');
        d.id = g + "_chartContainer";
        $("div#ChartsRow").append(d);
        $("div#" + g + "_chartContainer").addClass('genreChart');
        $("div#" + g + "_chartContainer").addClass('m-sm-5');
        $("div#" + g + "_chartContainer").append("<p><a id='showSeriesDetails' role='button' class='text-center btn btn-link' href='/genres/" + g + "'>" + g + "</a></p>");
        $("div#" + g + "_chartContainer").append(cvs);
        $("div#" + g + "_chartContainer").append("<div class='d-flex justify-content-between small'><span>1</span><span>10</span></div>");


        dps.map((dp) => {
            if (maxDp < dp.RatingCount) {
                maxDp = dp.RatingCount;
            }
        });
        var heightStep = chartHeight / maxDp;
        c.beginPath();
        c.lineWidth = gLineWidth;
        c.moveTo(0, chartHeight);
        c.lineCap = "round";


        dps.map((dp) => {
            var rating = dp.Rating;
            var ratingCount = dp.RatingCount;
            var x = rating * (chartWidth / 10);
            var y = chartHeight - (ratingCount * heightStep);
            var strokeColor = dp.Rating <= 6 ? gRatingColors[0] : gRatingColors[(Math.floor(dp.Rating) - 5)];
            c.strokeStyle = strokeColor;
            c.lineTo(x, y);
            c.stroke();
            c.beginPath();
            c.moveTo(x,y);
        });

    });
}

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip()
    var sl = fGetLS('genreChartData');
    if (!sl) {
        $.getJSON('/genreChartData.json', function (x) {
            fSetLS('genreChartData', JSON.stringify(x), 86400000);
            fSL(x);
        });
    }
    else {
        fSL(JSON.parse(sl));
    }
});
$(document).on("click", "table#GenreRankingTable tbody tr", function () {
    window.location = $(this).data('href');
});

const groupBy = key => array =>
    array.reduce((objectsByKeyValue, obj) => {
        const value = obj[key];
        objectsByKeyValue[value] = (objectsByKeyValue[value] || []).concat(obj);
        return objectsByKeyValue;
    }, {});
