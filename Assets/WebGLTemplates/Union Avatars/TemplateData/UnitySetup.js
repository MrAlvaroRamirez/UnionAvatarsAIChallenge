var canvas = document.querySelector("#unity-canvas");
var config = {
    dataUrl: "Build/{{{ DATA_FILENAME }}}",
    frameworkUrl: "Build/{{{ FRAMEWORK_FILENAME }}}",
    codeUrl: "Build/{{{ CODE_FILENAME }}}",
    //#if MEMORY_FILENAME
    memoryUrl: "Build/{{{ MEMORY_FILENAME }}}",
    //#endif
    //#if SYMBOLS_FILENAME
    symbolsUrl: "Build/{{{ SYMBOLS_FILENAME }}}",
    //#endif
    streamingAssetsUrl: "StreamingAssets",
    companyName: "{{{ COMPANY_NAME }}}",
    productName: "{{{ PRODUCT_NAME }}}",
    productVersion: "{{{ PRODUCT_VERSION }}}",
};
var scaleToFit;
try {
    scaleToFit = !!JSON.parse("{{{ SCALE_TO_FIT }}}");
} catch (e) {
    scaleToFit = true;
}
function progressHandler(progress) {
    var percent = progress * 100 + '%';
    canvas.style.background = 'linear-gradient(to right, white, white ' + percent + ', transparent ' + percent + ', transparent) no-repeat center';
    canvas.style.backgroundSize = '100% 1rem';
}
function onResize() {
    var container = canvas.parentElement;
    var w;
    var h;

    if (scaleToFit) {
        w = window.innerWidth;
        h = window.innerHeight;

        //var r = {{{ HEIGHT }}} / {{{ WIDTH }}};
        //
        //if (w * r > window.innerHeight) {
        //	w = Math.min(w, Math.ceil(h / r));
        //}
        //h = Math.floor(w * r);
    } else {
        w = "{{{ WIDTH }}}";
        h = "{{{ HEIGHT }}}";
    }

    container.style.width = canvas.style.width = w + "px";
    container.style.height = canvas.style.height = h + "px";
    container.style.top = Math.floor((window.innerHeight - h) / 2) + "px";
    container.style.left = Math.floor((window.innerWidth - w) / 2) + "px";
}

var gameInstance
createUnityInstance(canvas, config, progressHandler).then(function (instance) {
    canvas = instance.Module.canvas;
    onResize();
    gameInstance = instance;
});

window.addEventListener('resize', onResize);
onResize();

if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
    const meta = document.createElement('meta');
    meta.name = 'viewport';
    meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes';
    document.getElementsByTagName('head')[0].appendChild(meta);
}