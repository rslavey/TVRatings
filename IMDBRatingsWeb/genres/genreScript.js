let gRatingColors = ["#f8696b", "#fbaa77", "#ffeb84", "#b1d580", "#63be7e"];
let gEpsRankedCount = 3;
let gPathName = window.location.pathname.replace('/genres/', '').replace(/\//g, '');

function fLd() {

    document.title = 'What To Watch On TV -- Genre Ranking List -- ' + gPathName;

    var keywords = [];
    keywords.push(gPathName, "tv", "television", "imdb", "genre");

    document.getElementsByTagName('meta')["keywords"].content = keywords.join(",");
    document.getElementsByTagName('meta')["description"].content = "Genre List of Television Shows by Rating -- " + gPathName;

    $("#SeriesTitle").html("Rankings by Genre -- " + gPathName);
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
    if (!item.expiry){
        return null;
    }

    const now = new Date();

    if (now.getTime() > item.expiry) {
        localStorage.removeItem(key);
        return null;
    }
    return item.value;
}

function fSoL (a, b){
    var aRank = a.Genres.filter((s) => {
        return s.GenreName == gPathName;
    })[0].Rank;
    var bRank = b.Genres.filter((s) => {
        return s.GenreName == gPathName;
    })[0].Rank;

    return aRank > bRank ? 1 :
        aRank < bRank ? -1 : 0
}

function fSL(sl) {
    var fsl = sl.filter((s) => {
        return s.Genres.filter((sg) => {
            return sg.GenreName == gPathName && sg.Rank > 0;
        }).length > 0;
    })
        .sort(fSoL);

    fsl.map((s) => {
        var fGr = s.Genres.filter((s) => {
            return s.GenreName == gPathName;
        });
        var sGr = fGr[0].Rank;
        var nr = "<tr data-href='/" + s.CleanTitle + "' class='rating" + Math.floor(s.Rating) + "'><td>" + sGr + "</td><td>" + s.Title + "</td><td>" + s.Rating.toFixed(2) + "</td></tr>";
        $("table#GenreRankingTable tbody").append(nr);
    });
}

$(document).ready(function () {
    fLd();
    $('[data-toggle="tooltip"]').tooltip()
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
});
$(document).on("click", "table#GenreRankingTable tbody tr", function () {
    window.location = $(this).data('href');
});
