navigator.sayswho= (function(){
    var N= navigator.appName, ua= navigator.userAgent, tem;
    var M= ua.match(/(opera|chrome|safari|firefox|msie)\/?\s*(\.?\d+(\.\d+)*)/i);
    if(M && (tem= ua.match(/version\/([\.\d]+)/i))!= null) M[2]= tem[1];
    M= M? [M[1], M[2]]: [N, navigator.appVersion,'-?'];
    return M;
})();

var thisBrowser= (function(){
    var name = navigator.sayswho[0].toLowerCase();
    return {
        isIE: name == 'msie',
        isOpera: name == 'opera',
        isChrome: name == 'chrome',
        isSafari: name == 'safari',
        isFirefox: name == 'firefox'
    }
})();

function joinUrls(){
    function joinTwo(a, b){
        if(!a){
            return b;
        }
        if(!b){
            return a;
        }
        if((a[a.length-1] == '/')){
            a = a.slice(0, -1);
        }
        if((b[0] == '/')){
            b = b.slice(1);
        }
       return a + '/' + b;
    }

    var res = '';
    [].slice.call(arguments).forEach(function (arg){
        res = joinTwo(res, arg);
    });
    return res;
}

function strcmp(a, b) {
    if (a.toString().toLowerCase() < b.toString().toLowerCase()) return -1;
    if (a.toString().toLowerCase() > b.toString().toLowerCase()) return 1;
    return 0;
}


function HtmlEncode(s)
{
    var el = document.createElement("div");
    el.innerHTML = s;
    s = el.innerHTML;
    delete el;
    return s;
}

function getActionButtons(){
    return [Msg.Password_reset, Msg.New_SIS_info_CSV, Msg.School_setup, Msg.Invite_users];
}

Object.extend = function(destination, source) {
  for (var property in source)
    destination[property] = source[property];
  return destination;
};

function getNoNull(value){
    return (value != null) ? value : '';
}

function daysInMonth(month,year) {
    return new Date(year, month, 0).getDate();
}

function getSerial(number){
    switch(number){
        case 1:;
        case 21:;
        case 31: return number + 'st';
        case 2:;
        case 22: return number + 'nd';
        case 3:;
        case 23: return number + 'rd';
        default: return parseInt(number, 10) + 'th';
    }
}

function buildShortText(text, newTextlength){
    newTextlength = newTextlength || 80;
    if(text != null && text.length > newTextlength){
        return text.slice(0, newTextlength) + '...';
    }
    return text;
}

String.prototype.capitalize = function() {
    return this.charAt(0).toUpperCase() + this.slice(1);
}

function debugPrint(name, obj){
    console.log(name, 'start');
    for (var prop in obj){

        if (obj.hasOwnProperty(prop) && prop.indexOf('get') != -1){
            console.log(prop, obj[prop]());
        }
    }
    console.log(name, 'end');
}

$.validationEngineLanguage.allRules["double"] = {
    "regex": /^[-+]?([0-9]*\.[0-9]+|[0-9]+)$/,
    "alertText" : "Invalid value"
}