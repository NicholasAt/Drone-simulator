mergeInto(LibraryManager.library, {
    GetBrowserName: function () {
        var ua = navigator.userAgent;
        var browser = "Unknown";

        if (ua.indexOf("Edg/") > -1 || ua.indexOf("Edge/") > -1) {
            browser = "Edge";
        } else if (ua.indexOf("Chrome/") > -1 && ua.indexOf("Safari/") > -1) {
            browser = "Chrome";
        } else if (ua.indexOf("Safari/") > -1 && ua.indexOf("Chrome/") === -1) {
            browser = "Safari";
        } else if (ua.indexOf("Firefox/") > -1) {
            browser = "Firefox";
        }

        var bufferSize = lengthBytesUTF8(browser) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(browser, buffer, bufferSize);
        return buffer;
    },

    FreeMemory: function (ptr) {
        _free(ptr);
    },

    IsSafari: function () {
        var ua = navigator.userAgent;
        return (ua.indexOf("Safari/") > -1 && 
                ua.indexOf("Chrome/") === -1 && 
                ua.indexOf("Edg/") === -1) ? 1 : 0;
    }
});