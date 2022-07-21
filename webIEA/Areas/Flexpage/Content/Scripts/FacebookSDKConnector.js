
function initFaceBookAsync(appId, apiVersion) {
    window.fbAsyncInit = function () {
        FB.init({
            appId: appId,
            autoLogAppEvents: true,
            xfbml: true,
            version: apiVersion
        });
        window.isFacebookInit = true;
    };

    // connect facebook sdk
    (function (d, s, id) {
        var js, fjs = d.getElementsByTagName(s)[0];
        if (d.getElementById(id)) { return; }
        js = d.createElement(s); js.id = id;
        js.src = "https://connect.facebook.net/en_US/sdk.js#xfbml=1&version=" + apiVersion + "&appId=" + appId + "&autoLogAppEvents=1";
        fjs.parentNode.insertBefore(js, fjs);
    }(document, 'script', 'facebook-jssdk'));
}

function fbEnsureInit(callback) {
    if (!window.isFacebookInit) {
        setTimeout(function () { fbEnsureInit(callback); }, 50);
    } else {
        if (callback) {
            callback();
        }
    }
}
