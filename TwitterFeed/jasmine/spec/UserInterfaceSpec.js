 describe('highlight_html', function() {
  var sample_html_blob;

  beforeEach(function(){
    sample_html_blob = `<p>The quick brown fox jumps over the lazy dog</p>`;
  })

  it("returns the original blob if nothing to highlight", function() {
    expect(highlight_html(sample_html_blob), '').toBe(sample_html_blob);
    expect(highlight_html(sample_html_blob), null).toBe(sample_html_blob);
    expect(highlight_html(sample_html_blob), undefined).toBe(sample_html_blob);
  });

  it("highlight the right word", function() {
    expect(highlight_html(sample_html_blob, 'lazy')).toContain("<span class='highlighted'>lazy</span>");
  });


  it("doesn't highlight the characters in the tags", function() {
    expect(highlight_html(sample_html_blob, 'p')).not.toContain("<<span class='highlighted'>p</span>>");
  });
});

describe("twitter", function() {
  beforeEach(function() {
    jasmine.clock().install();
  });

  afterEach(function() {
    jasmine.clock().uninstall();
  });

  it("queries twitter every 60 seconds", function() {
    spyOn(window, 'query_twitter');
    twitter();

    expect(query_twitter.calls.count()).toEqual(1);

    jasmine.clock().tick(60000);
    expect(query_twitter.calls.count()).toEqual(2);

    jasmine.clock().tick(60000);
    expect(query_twitter.calls.count()).toEqual(3);
  });
});

describe('handle_success', function() {
  var sample_tweet = {
        created: '',
		image:'',
		media:'',
		name:'',
		retweet:'',
		screenname:'',
		text:''
        };

  it("should set global tweets to max 10 results", function() {
    spyOn(window, 'get_tweets').and.callThrough();
    var data = {
      tweets: Array.apply(null, Array(20)).map(function(){
        return sample_tweet;
      })
    }

    handle_success(data);
    expect(get_tweets().length).toEqual(10);
  });

  it("should handle correctly if there are less than 10 tweets", function() {
    spyOn(window, 'get_tweets').and.callThrough();
    var data = {
      tweets: Array.apply(null, Array(3)).map(function(){
        return sample_tweet;
      })
    }

    handle_success(data);
    expect(get_tweets().length).toEqual(3);
  });

});


describe('filter_tweets', function() {
  var tweets;
  beforeEach(function() {
    spyOn(window, 'get_tweets_shown').and.callThrough();
    var sample_tweet = {
        created: '',
		image:'',
		media:'',
		name:'',
		retweet:'',
		screenname:'',
		text:''
        };

    tweets = Array.apply(null, Array(3)).map(function(){
      return sample_tweet;
    })
    tweets.push({ user: { name: '', screen_name: '', profile_image_url: '', }, text: 'find me here', retweet_count: 0 });
  });

  it("should set global tweets_shown to right number of results", function() {
    filter_tweets(tweets, 'find me');
    expect(get_tweets_shown().length).toEqual(1);
  });

  it("should set tweets_shown to all tweets if there is no query", function() {
    filter_tweets(tweets, '');
    expect(get_tweets_shown()).toEqual(tweets);
  });
}); 