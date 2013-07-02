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

function HtmlEncode(s)
{
    var el = document.createElement("div");
    el.innerHTML = s;
    s = el.innerHTML;
    delete el;
    return s;
}

