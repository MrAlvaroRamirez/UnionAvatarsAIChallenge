mergeInto(LibraryManager.library, {
  
  LoadIFrame : function (targetObjectName) {
      const iframe = document.createElement("iframe");
      //Replace this URL with your IFrame URL
      iframe.src = "https://photobooth.unionavatars.com";
      iframe.id = "UnionIFrame";
      iframe.allow = "camera *;";
      document.body.appendChild(iframe);
      
      var convertedName = UTF8ToString(targetObjectName) + "";

      window.addEventListener('message', (e) => OnReceiveAvatarMessage(e, convertedName));
  }
});