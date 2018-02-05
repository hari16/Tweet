var tweets; 
var tweets_shown;

$(function () {

    twitter();

    $('#tweet_filter_query').keyup(function () {
        delay(function () {
            var str_query = $('#tweet_filter_query').val();
            filter_tweets(tweets, str_query);
            var html_blob = highlight_html(build_html(tweets_shown), str_query);
            render(html_blob);
        }, 600);
    });
});


function twitter() {
    query_twitter();
    var i = setInterval(function () {
        query_twitter();
    }, 60 * 1000);
};

var delay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();

function query_twitter(str_query) {

    $.ajax({
        cache: false,
        url: '/Home/GetFeed',
        method: 'GET',
        dataType: 'json',
        success: function success(data) {
            handle_success(data);
        },
        error: function error(response) {
            alert(response);
        }
    });
};

//event handlers
function handle_success(data) {
    tweets = data;
    tweets_shown = tweets;
    render(build_html(tweets_shown));
};

//html renders
function render(html_blob) {
    $('div#response').html(html_blob);
};

function highlight_html(html_blob, highlight_str) {
    if (highlight_str == '') {
        return html_blob;
    }

    //using regex lookahead
    var regex = new RegExp('(?![^<>]*>)(' + highlight_str + ')', 'ig');
    return html_blob.replace(regex, "<span class='highlighted'>" + highlight_str + "</span>");
};

function build_html(tweets) {
    var lis = tweets.map(build_html_widget).join('');

    return '<ul class="mdl-list">' + lis + '</ul>';
};

function build_html_widget(tweet) {

    return '<li class="tweet-list mdl-list__item"> <div class="tweet-card mdl-card mdl-shadow--6dp">  <span> <i> <img src=' + tweet.image + '> </i> <span> ' + tweet.name + ' </span>  <span> ' + '@' + tweet.screenname + ' </span> </span> </br> <span class="mdl-list__item-text-body"> ' + tweet.text + '<img class="tweet-media" src=' + tweet.media + ' align="center" onerror="this.remove();"/> </span> </br> <div class="mdl-card__actions mdl-card--border"> <span><i class="material-icons md-dark">cached</i>' + tweet.retweet + '</div></div>    </div>            </li>';
};

//Filter tweets using underscore.js
function filter_tweets(tweets, str_query) {
    tweets_shown = _.filter(tweets, function (el) {
        return el.text.includes(str_query);
    });
};

//for testing..
function get_tweets() {
    return tweets;
};

function get_tweets_shown() {
    return tweets_shown;
};