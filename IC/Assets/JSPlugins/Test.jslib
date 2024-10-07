mergeInto(LibraryManager.library, {

  HelloString: function (str) {
    window.alert(UTF8ToString(str));
  },

  GetURLParams: function () {
    let str =  document.location.search;
    let bufferSize = lengthBytesUTF8(str) + 1;
    let buffer = _malloc(bufferSize);
    stringToUTF8(str, buffer, bufferSize);
    return buffer;
  },

});