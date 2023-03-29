const OnReceiveAvatarMessage = function (event, targetObjectName) {
    //Compare message origin with the IFrame origin. We remove the last slash of the source to match the urls correctly
    if(event.origin == document.getElementById("UnionIFrame").src.replace(/\/+$/, ''))
    {
        console.log("Avatar data received! Sending it to unity...");

        gameInstance.SendMessage(targetObjectName, "ReceiveAvatarData", event.data);

        document.getElementById("UnionIFrame").remove();	
    }
}