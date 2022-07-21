$(document).ready(function () {
    //delete loggedout session cookie

    if (isUserLoggedIn == true) {
        delete_cookie('loggedout', window.location.hostname);
        //console.log('request is authenticated, starting the monitoring...')
        initSessionMonitor();

        getLastActivityTime = true;
    }
});

//Load jQuery First
//How frequently to check for session expiration in milliseconds
//10000 - every 10 sec
var sess_pollInterval = 10000;
//How many minutes the session is valid for
//declared and assigned value in globaljs
//var global_sess_expirationSeconds = 30;
//How many minutes before the warning prompt
//var sess_warningSeconds = 120;

var sess_intervalID;
var CountDownTimerID;
var sess_lastActivity;
var actualCheckingValue = (global_sess_expirationSeconds - (global_sess_warningSeconds) + global_sess_serverCheckDelaySeconds);
var getLastActivityTime;
//60 seconds for expected network lag time

var countDownStartesFrom = 1;

function initSessionMonitor() {
    sess_lastActivity = new Date();
    setCookie();
    sessSetInterval();
    $(document).bind('keypress', function (ed, e) { sessKeyPressed(ed, e); });
    $(document).bind('click', function (ed, e) { sessKeyPressed(ed, e); });
    //console.log("session monitor initialized");
}
function sessSetInterval() {
    sess_intervalID = setInterval('sessInterval()', sess_pollInterval);
}
function sessClearInterval() {
    clearInterval(sess_intervalID);
}
function sessKeyPressed(ed, e) {
    //console.log("key was pressed");
    setTimeout(function () {
        //msg('getLastActivityTime -', getLastActivityTime);

        if (getLastActivityTime == true) {
            sess_lastActivity = new Date();
        }

    }, 0);
}
function sessLogOut() {
    $.ajax({
        url: '/session/checkauth',
        type: 'GET',
        async: false,
        cache: false,
        success: function (data) {
            if (data == "True") {
                sessKeyPressed(null, null);
                RestartSessionMonitor();
            }
            else {
                displayMessage("Warning: Your authentication has expired! The content displayed may be different and some functions are unavailable.");
            }
        },
        error: function () {
            displayMessage("The server is not responding.");
        } 
    });
}

function sessInterval() {
    //console.log("session interval function called");
    var diffSec = GetTimeDifference();
    //console.log("current time difference is: " + diffSec + " actual checking value is: " + actualCheckingValue);
    //if session timeout value is 1200 sec then warning should start when the time diff is (1200-120-60)sec and will start a timer for 120 sec after that it will get automatically logout, before 60 sec of actual session timeout
    if (diffSec >= actualCheckingValue) {
        //wran before expiring
        //stop the timer
        sessClearInterval();
        //promt for attention        
        countDownStartesFrom = global_sess_warningSeconds;
        var msg = 'Warning! Your session will expire in ' + countDownStartesFrom + ' seconds.'
        displayMessage(msg);

        //stop getting latest activity time
        getLastActivityTime = false;

        CountDownTimerID = setInterval('CountDown()', 1000);
        //reset server session here
        clearInterval(actualsess_intervalID);
    }
}

function CancelLogout() {
    var diffSec = GetTimeDifference();

    clearInterval(CountDownTimerID);

    if (diffSec > global_sess_expirationSeconds) {

        //timed out
        sessLogOut();
    }
    else {
        RestartSessionMonitor();
    }
}

function RestartSessionMonitor() {
    hideMessage();
    //reset inactivity timer        
    sessSetInterval();
    sess_lastActivity = new Date();
    //msg('CancelLogout', sess_lastActivity);

    //There are two cases when its needed to restart the session monitor:
    //1 - user clicked a button on another tab. This implies that the session was updated, so we dont need to awake the server.
    //2 - checkAuth returned a value stating that the session is still active on the server. This is done with an AJAX request, which updates the session itself.
    //Either way it is not required to awake the server session manually.
    //awakeServerSession();

    //rerun this function to awake server session timeout
    getLastActivityTime = true;
}
function GetTimeDifference() {
    //if last activity is lesser than cookie's value then set the cookie value to last activity time
    var lastActivityFromCookie = getCookie();

    if (sess_lastActivity < lastActivityFromCookie) {
        sess_lastActivity = lastActivityFromCookie;
    }

    var diff = (new Date() - sess_lastActivity);
    var diffSec = (diff / 1000);
    return diffSec;
}

function CountDown() {
    //scenario- when 2 screens opened in 2 tabs, in one screen start showing warning box at that same time user is working in another screen
    //then cookie's lastupdated date will be updated by current screen so get that value and check if sess_lastActivity's date is lesser than that value
    //if so that means variable contains old value but last activity performed just now. In this scenario hide the warning box and cancel this countdown
    var lastActivityFromCookie = getCookie();
    //msg('getLastActivityTime - ', getLastActivityTime);
    //msg('lastActivityFromCookie - ', lastActivityFromCookie);
    //msg('sess_lastActivity - ', sess_lastActivity);

    if (lastActivityFromCookie > sess_lastActivity) {
        sess_lastActivity = lastActivityFromCookie;
        clearInterval(CountDownTimerID);
        hideMessage();
        //stop timer hide msg box
        //console.log("cancelling logout");
        CancelLogout();
    }
    else {
        countDownStartesFrom = countDownStartesFrom - 1;

        if (countDownStartesFrom <= 0) {
            clearInterval(CountDownTimerID);
            sessLogOut();
        }
        else {
            var msg = 'Warning! Your session will expire in ' + countDownStartesFrom + ' seconds.';
            displayMessage(msg);
        }
    }
}


//scenario: if a user open a form page and spend a long time to fill the form then from client side session will be active as he is doing something on the page
//but from server side application session will be timed out
//prevention: we can start a timer at page load whose tick time will be (session timeout value-60) sec. then before actual session timeout timer can
//call a function which can call ajax to rejuvenate server side session. Considering 60sec as lag time 

var actualsess_intervalID;
//this function will initiate a timer which will execute every (sessionTimeOut-60)*1000 millisec
function awakeServerSession() {
   // console.log("renewing server session");
    var oldgetLastActivityTime = getLastActivityTime;

    $.ajax({ url: global_sess_resetPage, type: "POST", cache: false, async: false });

    //reset old value after ajax call
    getLastActivityTime = oldgetLastActivityTime;
}

function setCookie() {
    //in every 2 sec get the last activity time and store that in cookie
    setInterval(function () {
        set_cookie('lastupdated', sess_lastActivity.toString(), 1*60*60*24, window.location.hostname);
    }, 2000)
}

function getCookie() {
    //when timed out from other tab and logged out there then force logout 
    if (get_cookie('loggedout') == 'true') {
        sessLogOut();
    }
    else {
        var date = new Date(get_cookie('lastupdated'));
        if (isNaN(date.getTime())) {  // checking if date is valid
            // date is not valid
        } else {
            return date;
        }
    }
}

function deleteCookie() {
    delete_cookie('lastupdated', window.location.hostname);
}

//to be called from application's logout button click
function deleteSession() {
    set_cookie('loggedout', 'true', 1 * 60 * 60 * 24, window.location.hostname);
}

function msg(prefix, val) {
    console.log(prefix + '- ' + val.toString());
}