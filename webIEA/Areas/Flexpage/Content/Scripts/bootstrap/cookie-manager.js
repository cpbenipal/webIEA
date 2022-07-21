//Generic cookie access functions
function set_cookie(cookie_name, cookie_value, lifespan_in_seconds, valid_domain) {
    setTimeout(function () {
        // http://www.thesitewizard.com/javascripts/cookies.shtml
        var domain_string = valid_domain ? ("; domain=" + valid_domain) : '';
        document.cookie = cookie_name + "=" + encodeURIComponent(cookie_value) +
            "; max-age=" +lifespan_in_seconds +
            "; path=/" + domain_string;
    }, 0);
}

function get_cookie(name) {
    //http://stackoverflow.com/questions/10730362/get-cookie-by-name
    var match = document.cookie.match(new RegExp('(^| )' + name + '=([^;]+)'));
    if (match) return decodeURIComponent(match[2]);
}

function delete_cookie(cookie_name, valid_domain) {
    // http://www.thesitewizard.com/javascripts/cookies.shtml
    var domain_string = valid_domain ? ("; domain=" + valid_domain) : '';
    document.cookie = cookie_name + "=; max-age=0; path=/" + domain_string;
}