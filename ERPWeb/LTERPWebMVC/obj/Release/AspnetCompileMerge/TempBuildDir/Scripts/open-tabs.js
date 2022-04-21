(function ($, window, document, undefined) {
    'use strict'; 
    var pluginName = 'iframes';
    var thisElement = '';
    var height = '500px';//以这种形式出现的iframe,必须设置高度,不能设置100%
    //入口方法
    $.fn[pluginName] = function (options) {
        var self = $(this);
        thisElement = self;
        if (this == null)
            return null;
        var data = this.data(pluginName);
        if (!data) {
            $(".loader").show();
            data = new BaseIframe(this, options);
            self.data(pluginName, data);
        }
        return data;
    };


    var BaseIframe = function (element, options) {
        this.$element = $(element);
        this.options = $.extend(true, {}, this.default, options);
        this.init();
    }

    //默认配置
    BaseIframe.prototype.default = {
        showIndex: 0, //默认显示页索引
        loadAll: false//true=一次全部加在页面,false=只加在showIndex指定的页面，其他点击时加载，提高响应速度
    }

    //结构模板
    BaseIframe.prototype.template = {
        div_tabs: '<div class="row content-tabs"></div>',
        nav_menuTabs: '<nav class="page-tabs J_menuTabs"></nav>',
        nav_a: '<a href="javascript:;" class="J_menuTab" data-id="{0}" data-index="{1}" name="{2}" onfocus="this.blur();">{3}</a>',//这里不能用id,id会追加代码到a标签内
        div_content: '<div class="page-tabs-content" id="page-tabs"></div>',
        div_iframe: '<div class="row J_mainContent" id="content-tabs"></div>',
        iframe: '<iframe onload="onloadhandler(this);" class="J_iframe" name="{0}" data-index="{1}" width="100%" height="{2}" src="{3}" frameborder="0" data-id="{4}" seamless style="display: none;"></iframe>',
        a_close: '<i class="fa fa-remove" title="关闭" data-id="{0}"></i>'
    }
    
    //初始化
    BaseIframe.prototype.init = function () {
        if (!this.options.data || this.options.data.length == 0) {
            console.error("请指定tab页数据");
            return;
        }
        //当前显示的显示的页面是否超出索引
        if (this.options.showIndex < 0 || this.options.showIndex > this.options.data.length - 1) {
            console.error("showIndex超出了范围");
            //指定为默认值
            this.options.showIndex = this.default.showIndex;
        }
        if (this.options.height) {
            height = this.options.height;
        }
        //清除原来的tab页
        this.$element.html("");
        this.builder(this.options.data);
    }

    //使用模板搭建页面结构
    BaseIframe.prototype.builder = function (data) {
        var div_tabs = $(this.template.div_tabs);
        var nav_menuTabs = $(this.template.nav_menuTabs);
        var div_content = $(this.template.div_content);

        var div_iframe = $(this.template.div_iframe);
        for (var i = 0; i < data.length; i++) {
            //nav-tab
            var nav_a = $(this.template.nav_a.format(data[i].url, i, data[i].id, data[i].text));
            //如果可关闭,插入关闭图标，并绑定关闭事件
            if (data[i].closeable) {
                var a_close = $(this.template.a_close.format(data[i].id));

                nav_a.append("&nbsp;");
                nav_a.append(a_close);
            }

            div_content.append(nav_a);


        }
        nav_menuTabs.append(div_content);
        div_tabs.append(nav_menuTabs);
        this.$element.append(div_tabs);
        this.$element.append(div_iframe);
        this.loadData();
    }

    /**
     * 添加Iframe
     * */
    BaseIframe.prototype.openIframe = function (dataUrl, menuName, dataIndex) {
        //console.log("openiframe",this);
        // 获取标识数据
        var flag = true;
        if (dataUrl == undefined || $.trim(dataUrl).length == 0) return false;
        thisElement.find("#content-tabs iframe").each(function () {
            //console.log('id',$(this).data('id'));
            //console.log('url',dataUrl);
            if ($(this).data('id') == dataUrl) {

                flag = false;
            }
        });
        
        if (flag) {
            //不存在iframe，创建iframe
            $(".loader").show();
            var iframe = $(this.template.iframe.format(menuName, dataIndex, height, dataUrl, dataUrl));

            thisElement.find("#content-tabs").append(iframe);
        } else {
            //存在iframe，根据data值显示或隐藏
           // $(ifrm).data("isloaded", 1);
           var _isloadv= $("iframe[name=" + menuName+ "]").data("isloaded");
            if (_isloadv == 1 || _isloadv == "1") {
                $(".loader").hide();
            } else {
                $(".loader").show();
            }
        }
        BaseIframe.prototype.hidden_a(menuName);
        return false;
    }

    /**加载数据*/
    BaseIframe.prototype.loadData = function () {
        var data = this.options.data;
        //如果是当前页或者配置了一次性全部加载，否则点击tab页时加载
        for (var i = 0; i < data.length; i++) {
            if (this.options.loadAll || this.options.showIndex == i) {
                if (data[i].url) {
                    // 添加iframe
                    var iframe = $(this.template.iframe.format(data[i].id, i, height, data[i].url, data[i].url));
                    this.$element.find("#content-tabs").append(iframe);
                } else {
                    console.error("id=" + data[i].id + "的iframe页未指定url");
                }
            }
        }
        //console.log("this++",this);
        //console.log("thisdafult++",BaseIframe.prototype.default);
        if (data.length > 1) {
            this.$element.find("#content-tabs iframe").eq(this.options.showIndex).css("display", "inline");
            this.$element.find('#page-tabs a').eq(this.options.showIndex).addClass('active');
        } else {
            this.$element.find("#content-tabs iframe").eq(0).css("display", "inline");
            this.$element.find('#page-tabs a').eq(0).addClass('active');
        }
        //绑定事件
        BaseIframe.prototype.eventses();
        //$("#content-tabs iframe").css("height",height);
        //console.log(thisElement.attr('id'));
        //console.log(thisElement.html());
    }

    /**点击事件*/
    BaseIframe.prototype.eventses = function () {
        //console.log("thisElement",thisElement);
        thisElement.find("#page-tabs a i").each(function () {
            $(this).click(function () {
                var id = $(this).data('id'); 
                //console.log('ad0',id);
                BaseIframe.prototype.remove(id);
            });
        });
        thisElement.find("#page-tabs a").click(function () {
            //console.log("点击a: ",this);
            var dataUrl = $(this).data('id');
            var menuName = $(this).text();
            var name = $(this).attr('name');
            var dataIndex = $(this).data('index');;
            BaseIframe.prototype.openIframe(dataUrl, name, dataIndex);
        });

    }

    //新增一个Iframe页
    BaseIframe.prototype.addIframe = function (obj) {
        //判断是否已经存在
        if (thisElement.find("#page-tabs a[name=" + obj.id + "]").length == 0)
        { 
            var nav_a = $(this.template.nav_a.format(obj.url, $('#page-tabs a').length, obj.id, obj.text));
            //如果可关闭,插入关闭图标，并绑定关闭事件
            if (obj.closeable) {
                var a_close = $(this.template.a_close.format(obj.id)); 
                nav_a.append("&nbsp;");
                nav_a.append(a_close);
            } 
            thisElement.find("#page-tabs").append(nav_a); 
            BaseIframe.prototype.eventses();
        }
        //打开对应的tab
        var addObj=thisElement.find("#page-tabs a[name=" + obj.id + "]");
        var dataUrl = addObj.data('id');
        var menuName = addObj.text();
        var name = addObj.attr('name');
        var dataIndex = addObj.data('index');;
        BaseIframe.prototype.openIframe(dataUrl, name, dataIndex);
    }

    /**隐藏其他的,显示当前*/
    BaseIframe.prototype.hidden_a = function (obj) {
        thisElement.find('#page-tabs a').each(function () {
            $(this).removeClass('active');
        });
        thisElement.find('#content-tabs iframe').each(function () {
            $(this).css("display", "none");
        });
        thisElement.find("iframe[name='" + obj + "']").css("display", "inline");
        thisElement.find("a[name='" + obj + "']").addClass('active');
        //console.log("height",height);
        //thisElement.find("#content-tabs iframe").css("height",height);
    }

    //根据id获取活动也标签名
    BaseIframe.prototype.find = function (tabId) {
        return this.$element.find(".nav-tabs li a[href='#" + tabId + "']").text();
    }

    // 删除活动页
    BaseIframe.prototype.remove = function (obj) {
        thisElement.find("#content-tabs iframe[name='" + obj + "']").remove();
        thisElement.find("#page-tabs a[name='" + obj + "']").remove();
        thisElement.find('#page-tabs a').eq(0).addClass('active');
        thisElement.find("#content-tabs iframe").eq(0).css("display", "inline");
        if (thisElement.find('#page-tabs a').length > 0) {
            var a = thisElement.find('#page-tabs a').eq(0);//找到第一个a标签取出数据
            this.openIframe(a.data('id'), a.attr('name'), a.data('index'));
        }
    }

    //根据id设置活动iframe页
    BaseIframe.prototype.showIframe = function (tabId) {
        var a = thisElement.find("#page-tabs a[name='" + tabId + "']");//找到a标签
        //console.log("a标签",a);
        var dataUrl = a.data('id');
        var dataIndex = a.data('index');;
        BaseIframe.prototype.openIframe(dataUrl, tabId, dataIndex);
    }

    //获取当前活动iframe页的ID
    BaseIframe.prototype.getCurrentIframeId = function () {
        var id = thisElement.find("#page-tabs .active a").attr("name");
        console.log('id', id);
        return id;
    }

    String.prototype.format = function () {
        if (arguments.length == 0) return this;
        for (var s = this, i = 0; i < arguments.length; i++)
            s = s.replace(new RegExp("\\{" + i + "\\}", "g"), arguments[i]);
        return s;
    };
})(jQuery, window, document)

function onloadhandler(ifrm) {
    $(ifrm).data("isloaded", 1);
   // alert($("iframe[name=" + $(ifrm).attr("name") + "]").data("isloaded"));   
    $(".loader").hide();
}