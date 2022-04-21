//打印物料条码
function LocalPrintMatterBarcode(prdName,prdBarcode,postUrl)
{ 
    $.post(postUrl, { m_name: prdName, m_barcode: prdBarcode}, function (json) {
        json = json || {};
        if (json.success) { 
            //  alert("打印成功！");
            console.log(json.data);
        } else {
            alert(json.errors);
        } 
    }).fail(function () {
        alert("操作失败！");
    });
}
//语音
function LT_Speech() {  
    //以下代码可用
    /*//采用h5直接调web speech api （ie不支持）
    this.speek = function (textToSpeak) {
        try {
            //创建一个 SpeechSynthesisUtterance的实例
            var newUtterance = new SpeechSynthesisUtterance();
            // 设置文本
            newUtterance.text = textToSpeak;
            //播放
            //window.speechSynthesis.speak();
            //暂停
           // window.speechSynthesis.pause();
            //继续
           // window.speechSynthesis.resume();
            //停止
            window.speechSynthesis.cancel(); 
            // 添加到队列
            window.speechSynthesis.speak(newUtterance);
        } catch (exx) {
            console.log(exx);
        }
    };
    /////////////////////////////////////
    */
    //****以下代码可用。。。
    this.spcId = "LtSpc_EmbedPlay";
    this.lt_speech_src = "/voicehandler.ashx?voice=";
    this.speek = function (text) {
        if (IEVersion() >= 0) {
            //ie embed
            //IE内核浏览器
            this.addEmbedSpeech(text);
        } else {
            //非ie audio
            this.addEmbedSpeech2(text);
        }
    };
    this.addEmbedSpeech = function (src) {
        if ($("#" + this.spcId).length > 0) {
            $("#" + this.spcId).get(0).stop();
            $("#" + this.spcId).remove();
        }
        $("body").append('<embed id="' + this.spcId + '" src="' + this.lt_speech_src + src + '" autostart="true"  hidden="true" loop="false" volume="100"/>');
    }
    this.addEmbedSpeech2 = function (src) { 
        if ($("#" + this.spcId).length > 0) { 
            $("#" + this.spcId).get(0).pause();
            $("#" + this.spcId).remove();
        } 
        $("body").append('<audio id="' + this.spcId + '" src="' + this.lt_speech_src + src + '" hidden="true" />');
        $("#" + this.spcId).get(0).play();
    }
   /// */
} 
//获取ie版本
function IEVersion() { 
    var userAgent = navigator.userAgent; //取得浏览器的userAgent字符串 
    //console.log(userAgent);
    /*
     *Edge: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)
            * Chrome/90.0.4430.72 Safari/537.36 Edg/90.0.818.42*/
    /*IE11:Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; .NET4.0C; 
             * .NET4.0E; .NET CLR 2.0.50727; .NET CLR 3.0.30729; .NET CLR 3.5.30729; rv:11.0) like Gecko */
    /*Firefox:Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0 */
    var isIE = userAgent.indexOf("compatible") > -1 && userAgent.indexOf("MSIE") > -1; //判断是否IE<11浏览器 
    var isEdge = userAgent.indexOf("Edg") > -1 && !isIE; //判断是否IE的Edge浏览器 
    var isIE11 = userAgent.indexOf('Trident') > -1 && userAgent.indexOf("rv:11.0") > -1; 
    if (isIE) { 
        var reIE = new RegExp("MSIE (\\d+\\.\\d+);"); 
        reIE.test(userAgent); 
        var fIEVersion = parseFloat(RegExp["$1"]); 
        if (fIEVersion == 7) { 
            return 7; 
        } else if (fIEVersion == 8) { 
            return 8; 
        } else if (fIEVersion == 9) { 
            return 9; 
        } else if (fIEVersion == 10) { 
            return 10; 
        } else { 
            return 6;//IE版本<=7 
        } 
    } else if (isEdge) { 
        return -1;//'edge';//edge 
    } else if (isIE11) { 
        return 11; //IE11 
    } else { 
        return -1;//不是ie浏览器 
    } 
} 
 
