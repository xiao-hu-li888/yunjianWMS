
//删除附件
function deleteannex(t, idstr) {
    try {
        var arr = idstr.split("|");
        if (arr.length > 1) {
            if (!confirm("确认删除？")) {
                return;
            }
            var id1 = parseInt(arr[0]);
            var id2 = parseInt(arr[1]);
            $.post("/Annex/DeleteAnnex/" + id1, {}, function (data) {
                if (data.success) {
                    alert("删除成功！");
                    $(t).parent().remove();
                } else {
                    alert('删除失败！请重试。');
                }
            }); 
        } else {
            alert("删除失败！参数错误。");
        }
    } catch (exx) { alert(exx); }
}
function LT_JS() {
    var isjkdoc = false;
    var clicknum = 0;
    /*表格点击监控*/
    this.TableClick = function () {
       // if ($(".btn_div2").length == 0) return;
        var isoperate = $(".btn_div2").is(".btn_div");
        if ($(".border").length > 0) {
            $(".border").each(function (a, b) {
                var cssclass = $(b).attr("selectedcssclass");
                if (cssclass && cssclass.length > 0) {
                    //操作浮动
                    if (isoperate) { 
                        $(b).find("tr").each(function (c, d) {                            
                            var top = $(d).find(".current_Data");                         
                                if (top.is("input")) {                                    
                                    $("<div class='lt_operate'>操作</div>").appendTo($(d).children().last()).click(function (event) {
                                        clicknum =1;
                                        if (!$("#border_value").is("input")) {
                                            $("body").append("<input type='hidden' value='' id='border_value' name='border_value'/>");
                                        }
                                        $("#border_value").val($(this).val());
                                        //显示
                                        var top = $(this).position().top;
                                        var left = $(this).position().left;
                                        $(".lt_operate").removeClass("lt_btn_over");
                                        $(this).addClass("lt_btn_over");
                                        var btnthis = $(".btn_div2");
                                        btnthis.css({ top: top + 19, left: left - btnthis.outerWidth() + $(this).outerWidth() }).show();  
                                    });                                    
                                }                             
                        });
                        if (!isjkdoc) {
                            isjkdoc = true;
                            //点击取消浮动
                            $(document).click(function (event) {
                                if (clicknum == 1) { clicknum = 0; return; }
                                $(".btn_div2").hide();
                                $(".lt_operate").removeClass("lt_btn_over");
                            });
                        }
                    }
                    $(b).find("tr").click(function () {
                        /*选中后赋值*/
                        var t = $(this).find(".current_Data");                        
                        if (t.is("input")) {
                            if (!$("#border_value").is("input")) {                             
                                $("body").append("<input type='hidden' value='' id='border_value' name='border_value'/>");
                            }                            
                            $("#border_value").val($(t).val());                            
                        } else { return; }
                        /*
                        if ($(b).find(".table_a_hidden").length == 0) {
                            $(b).find("td:eq(0)").append("<a href='#' class='table_a_hidden'>接受  </a>");
                        }
                        $(b).find(".table_a_hidden").focus();
                        */
                        $(b).find(".sele").removeClass("sele");
                        $(this).addClass("sele");
                    });
                    $(b).find("tr").dblclick(function () {
                        /*选中后赋值*/
                        var t = $(this).find(".current_Data");
                        if (t.is("input")) {
                            if (!$("#border_value").is("input")) {
                                $("body").append("<input type='hidden' value='' id='border_value' name='border_value'/>");
                            }
                            $("#border_value").val($(t).val());
                        } else { return; }
                        if ($(b).find(".table_a_hidden").length == 0) {
                            $(b).find("td:eq(0)").append("<a href='#' class='table_a_hidden'>接受</a>");
                        }
                        $(b).find(".table_a_hidden").focus();
                        $(b).find(".sele").removeClass("sele");
                        $(this).addClass("sele");
                        var durl = $(this).data("url"); 
                        if (durl&&durl.length > 0) {
                            if ($(this).data("open-new")) {
                                var text = $(this).data("dialog-title");
                                if (text == "") { text = $(this).text(); }
                                if (window.parent) {
                                    try {
                                        window.parent.window.AddAndOpenLinkToIframe(durl, text);
                                    } catch (e) { alert(e); }
                                    event.preventDefault();
                                }
                            } else if ($(this).data("open-dialog")) {
                                loadAndShowDialog("aa", $(this), durl);
                            }
                            else {
                                location.href = durl;
                            }
                        } else
                            $(".btn_div .ico_edit").eq(0).click();
                    });

                }
            });
        }
    }
    /*清除选择项*/
    this.ClearSelect = function () {
        if ($("#border_value").is("input") && $("#border_value").val().length > 0) {
            $("#border_value").val("");
        }
    }

    /*加载中显示*/
    this.loadBgShow = function () { 
        if ($(".loadview").is("div")) {
            $(".loadview").show();
        } else {
            $("<div class='loadview'><div class='bg'></div><div class='contload'></div></div>").appendTo("body");
            $(".loadview").show();
        }
    }
    this.loadBgHide = function () {
        $(".loadview").hide();
    }
    /*加载中隐藏*/

    this.printOpen = function (a) {
        if (window.parent!=window) {
            window.open("/Home/Print?p=" + window.decodeURI(a), "_blank");
        } else {
            window.print();
        }
    }
    this.RigesterDataOpen = function () {  
       $(".btn_div .icos,a[data-open=true]").off("click").on("click", function (event) {  
                //Dialog_Replacement_Id = "";
                //设置.nonea阻止点击 
                if ($(this).is(".nonea")) {
                    event.preventDefault();
                    return;
                }
                //过滤新窗口打开
                if ($(this).data("open-new")) {
                    return;
                }
                //刷新操作
                if ($(this).is(".ico_refresh")) {
                    reload();
                    return;
                }
                //返回链接
                if ($(this).is(".ico_back")) {
                    if ($(this).attr("href") == "#") {
                        window.history.back();
                        return;
                    }
                }

                //重设url
                if (setHref(this)) {
                    event.preventDefault();
                    return
                }
                var link = $(this), url = link.attr('href');
                /*对话框*/
                if (link.data('dialog-title')) {

                    if (link.data("annex-id") && link.data("annex-id").indexOf("addnewannex2") == 0) {
                        //    alert(link.data("annex-id"));
                        loadAndShowDialog(link.data("annex-id"), link, url);
                    } else
                        if (link.data("annex-id") && link.data("annex-id") == "addnewannex") {
                            loadAndShowDialog("addnewannex", link, url);
                        } else if (link.data("replacement-id")) {//replacement-id 提交数据后替换的Id               
                            var repid = link.data("replacement-id").replace("{0}", $("#border_value").val());
                            //alert(repid);
                            loadAndShowDialog("ReplaceDialog_" + repid, link, url);
                        } else {
                            loadAndShowDialog("aa", link, url);
                        }
                    event.preventDefault();
                    event.stopPropagation();  //只阻止了冒泡事件， 默认默认行为没有阻止 
                } else if (link.data("save")) {
                    if (!$("#ltListform").valid || $("#ltListform").valid()) {
                        ltJS.loadBgShow();
                        $.post(url, $("#ltListform").serializeArray()).done(function (json) {
                            ltJS.loadBgHide();
                            json = json || {};
                            if (json.success) {
                                ltLoad("");
                                //location = location.href;
                                // alert("操作成功");
                            } else if (json.errors) {
                                alert(json.errors);
                            }
                        }).error(function () {
                            ltJS.loadBgHide();
                            alert("操作过程中出现异常");
                        });
                    }
                    event.preventDefault();
                }
                else if (link.data("ltext-ajax")) {
                    event.preventDefault();
                    asyncRequest(this, {
                        url: $(this).attr("href"),
                        type: "GET",
                        data: []
                    });
                } else if (link.data("ltext-ajax-delete")) {
                    event.preventDefault();
                    if (confirm(link.data("ajax-confirm"))) {
                        ltJS.loadBgShow();
                        $.post(url, {}, function (json) {
                            ltJS.loadBgHide();
                            json = json || {};
                            if (json.success) {
                                alert("删除成功！");
                            } else {
                                alert(json.errors);
                            }
                            if (typeof (goto_page) == "function") {
                                goto_page(-1);
                            } else {
                                reload();
                            }
                        });
                    }
                    //errors
                }
            });
    }
    //全选 data-chkid="checkAll1" /data-chk="checkAll1" 
    $("input[type=checkbox][data-chkid]").each(function () { 
        var _datachkid = $(this).data("chkid"); 
        $(this).click(function () { 
            var _checkedV = $(this).get(0).checked; 
            $("input[type=checkbox][data-chk=" + _datachkid + "]").each(function () {
                $(this).get(0).checked = _checkedV;
            }); 
           // $("input[name='checkbox']").attr("checked", "true");
        })
    });
     
}

window.GetALLCheckedValue=function (chkid) {
    var _vals = "";
    $("input[type=checkbox][data-chk=" + chkid + "]").each(function () {
        if ($(this).get(0).checked) {
            if (_vals == "") {
                _vals = $(this).val();
            } else {
                _vals += "," + $(this).val();
            }
        }
    });
    return _vals;
}
var ltJS = new LT_JS();
/*重新加载表格*/
function ReLoadTable() { 
    ltJS.TableClick();
    ltJS.ClearSelect();
}
$(function () {

    ltJS.TableClick();
    //当前页面刷新处理
    window.ltLoad = function (hf) {
        if (location.href == hf || hf == "") {
            if (document.forms.length > 0) {
                for (var i = 0; i < document.forms.length; i++) {
                    var par = $(document.forms[i]);
                    if (par.parent().is(".searchbg")) {
                        par.submit();
                        return;
                    }
                }
            }
        }
        location.href = hf || location.href;
    }
    //监控表格中按键
    $(document).keypress(function (event) {
        switch (event.keyCode) {
            case 38://向上
                var btnd = $(".border .sele").prev();
                do {

                    var t = btnd.find(".current_Data");
                    if (t.is("input")) {
                        btnd.click();
                        break;
                    }
                    btnd = btnd.prev();
                } while (btnd.is("tr"))
                break;
            case 40://向下
                var btnd2 = $(".border .sele").next();
                do {

                    var t = btnd2.find(".current_Data");
                    if (t.is("input")) {
                        btnd2.click();
                        break;
                    }
                    btnd2 = btnd2.next();
                } while (btnd2.is("tr"))
                break;

                // $(".border .sele").next().click();
                break;
        }
    });
    /*错误提示处理*/
    var getValidationSummaryErrors = function ($form) {
        // We verify if we created it beforehand
        var errorSummary = $form.find('.validation-summary-errors, .validation-summary-valid');
        if (!errorSummary.length) {
            errorSummary = $('<div class="validation-summary-errors"><span>出现以下错误，请解决后重试。</span><ul></ul></div>')
                .prependTo($form);
        }

        return errorSummary;
    };
    //错误提示加载
    var displayErrors = function (form, errors) {
        var errorSummary = getValidationSummaryErrors(form)
            .removeClass('validation-summary-valid')
            .addClass('validation-summary-errors');
        var items = $.map(errors, function (error) {
            return '<li>' + error + '</li>';
        }).join('');

        var ul = errorSummary
            .find('ul')
            .empty()
            .append(items);
    };
    var dialogs = {};
    // var Dialog_Replacement_Id = "";
    //删除对话框
    var resetForm = function ($form) {
        // We reset the form so we make sure unobtrusive errors get cleared out.
        //   $form[0].reset();

        // getValidationSummaryErrors($form)
        //    .removeClass('validation-summary-errors')
        //    .addClass('validation-summary-valid')
        $form.remove();
    };
    //弹出框表单提交
    window.formSubmitHandler = function (e) { 
        var $form = $(this);
        // 判断是否存在大数据提交
        if ($form.attr("enctype") == "multipart/form-data") {
            ltJS.loadBgShow();
            var d = $form.attr("target");
            $("#" + d).load(function () { 
                var io = document.getElementById(d);
                var xml = {};
                try {
                    if (io.contentWindow) {
                        xml.responseText = io.contentWindow.document.body ? io.contentWindow.document.body.innerHTML : null;
                        xml.responseXML = io.contentWindow.document.XMLDocument ? io.contentWindow.document.XMLDocument : io.contentWindow.document;

                    } else if (io.contentDocument) {
                        xml.responseText = io.contentDocument.document.body ? io.contentDocument.document.body.innerHTML : null;
                        xml.responseXML = io.contentDocument.document.XMLDocument ? io.contentDocument.document.XMLDocument : io.contentDocument.document;
                    }
                } catch (e) {
                }                
                var data = xml.responseText; 
                var start = data.indexOf(">");
                if (start != -1) {
                    var end = data.indexOf("<", start + 1);
                    if (end != -1) {
                        data = data.substring(start + 1, end);
                    }
                } 
                var jdata = JSON.parse(data); 
                ltJS.loadBgHide();
                if (jdata.success) {
                    if (jdata.annexid > 0) {
                        if ($form.find("#Dialog_Attachment_Id").length > 0) {
                            //附件放置id
                            var _AttId = $form.find("#Dialog_Attachment_Id").val();
                            if ($("#" + _AttId).length > 0) {
                                if (dialogs[_AttId]) {
                                    dialogs[_AttId].dialog("close");
                                }
                                $("#" + _AttId).append("<div class='annexitem'><input type='hidden' name='annexid' value='" +
                                 jdata.annexid + "|" + jdata.refid + "'/>" +
                                 "<a class='ico_download2' href='/Annex/Index/" + jdata.guid + "'>" + jdata.annexname + "</a>" +
                                 "&nbsp;&nbsp;<a href='javascript:void(0);' onclick=\"deleteannex(this,'" +
                                 jdata.annexid + "|" + jdata.refid + "');\"><img src=\"/Content/ico/cross.png\" width=\"12\" height=\"12\" class=\"litle-ext-del\" /></a></div>");
                            } else {
                                ltLoad(jdata.redirect || location.href);
                            }
                        } else {
                            if ($("#ComAnnexDiv").length > 0) {
                                if (dialogs["addnewannex"]) {
                                    dialogs["addnewannex"].dialog("close");
                                }
                                $("#ComAnnexDiv").append("<div class='annexitem'><input type='hidden' name='annexid' value='" +
                                 jdata.annexid + "|" + jdata.refid + "'/>" +
                                 "<a class='ico_download2' href='/Annex/Index/" + jdata.guid + "'>" + jdata.annexname + "</a>" +
                                 "&nbsp;&nbsp;<a href='javascript:void(0);' onclick=\"deleteannex(this,'" +
                                 jdata.annexid + "|" + jdata.refid + "');\"><img src=\"/Content/ico/cross.png\" width=\"12\" height=\"12\" class=\"litle-ext-del\" /></a></div>");
                            } else {
                                ltLoad(jdata.redirect || location.href);
                            }
                        }
                    } else {
                        ltLoad(jdata.redirect || location.href);
                    }
                } else {
                    displayErrors($form, jdata.errors);
                }
                jQuery(io).unbind();
            });
            //  $form.submit();
            return true;
        }
        if (!$form.valid || $form.valid()) { 
            ltJS.loadBgShow();
            // if (Dialog_Replacement_Id && $.trim(Dialog_Replacement_Id) != "")   
            if ($form.find("#Dialog_Replacement_Id").length > 0) {//替换页面内容
                var DgId = $form.find("#Dialog_Replacement_Id").val();
                $.ajax({
                    type: "POST",
                    dataType: "html",
                    url: $form.attr('action'),
                    data: $form.serializeArray(),
                    success: function (html) {
                        ltJS.loadBgHide();
                        if (dialogs[DgId]) {
                            dialogs[DgId].dialog("close");
                        } 
                        if ($("#" + DgId.replace(/^ReplaceDialog_/, "")).length > 0) {
                            $("#" + DgId.replace(/^ReplaceDialog_/, "")).html(html);
                        } else {
                            location.href = location.href;
                        }
                    }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                        ltJS.loadBgHide();
                        alert("操作失败！");
                    }
                });
            } else {
                $.post($form.attr('action'), $form.serializeArray(), function (json) {
                    ltJS.loadBgHide();
                    json = json || {};
                    if (json.success) {
                        if (json.ajaxredirect) {//当前弹出窗口跳转
                            //  json.redirect
                            for (var d in dialogs) {
                                dialogs[d].dialog("close");
                            }
                            loadAndShowDialog("aa", $("<a data-dialog-width='800'></a>"), json.redirect);
                        } else {
                            alert("操作成功！");
                            $.addcomm.refreshLeftFrame();
                            ltLoad(json.redirect || location.href);
                            // alert(json.redirect || location.href);
                        }
                    } else if (json.errors) {
                        displayErrors($form, json.errors);
                    }
                }).fail(function () {
                    displayErrors($form, ['An unknown error happened.']);
                });
            }
        }
        e.preventDefault();
        e.stopPropagation(); 
    };
    //加载并弹出对话框
    window.loadAndShowDialog = function (id, link, url) {
        ltJS.loadBgShow();
        var separator = url.indexOf('?') >= 0 ? '&' : '?'; 
        dialogs[id] = $(); 
        $.get(url + separator + 'content=1&rd=' + Math.random(), function (content) { 
            ltJS.loadBgHide();
            dialogs[id] = $('<div class="modal-popup">' + content + '</div>')
                .hide() // Hide the dialog for now so we prevent flicker
                .appendTo(document.body)
                .filter('div') // Filter for the div tag only, script tags could surface
                .dialog({ // Create the jQuery UI dialog
                    title: link.data('dialog-title'),
                    modal: true,
                    bgiframe: link.data('dialog-iframe'),
                    resizable: true,
                    draggable: true,
                    width: link.data('dialog-width') || 600,
                    beforeClose: function () { resetForm($(this)); }
                })
                .find('form') // Attach logic on forms
                .submit(formSubmitHandler)
                .end();
            //("#Dialog_Replacement_Id").val()
            if (id.indexOf("ReplaceDialog_") == 0) { 
                dialogs[id].find("form").append("<input type='hidden' value='" + id + "' id='Dialog_Replacement_Id'>");
            }
            if (id.indexOf("addnewannex2") == 0) { 
                //附件地址
                dialogs[id].find("form").append("<input type='hidden' value='" + id + "' id='Dialog_Attachment_Id'>");
            }
            $.validator.unobtrusive.parse("form");
            DataAuto();
            if (link.data("change-parentheight") == 1) {
                try { 
                    window.parent.window.chgHeight();
                } catch (exc) {
                    //alert(exc);
                }
            }
        }).fail(function () {
            alert("加载失败，请重试！");
            ltJS.loadBgHide();
        });
        
        //, function () {
        //    alert("加载失败，请重试！");
        //    ltJS.loadBgHide();
        //}
        //$.get().error().done();
    };
    //加载或刷新页
    window.reload = function (href) {

        if (arguments[0]) {

            ltLoad(href);
            //  window.location.href = href;
        }
        else {
            ltLoad("");

        }
        //  window.location.reload();
    }
    //设置Href
    window.setHref = function (link) {
        var hf = $(link).attr("href");
        var oldhf = $(link).data("href");

        if (oldhf == undefined) {
            $(link).data("href", hf);
        } else {
            hf = oldhf;
        }
        if (hf.indexOf("{0}") >= 0) {
            if ($("#border_value").is("input") && $("#border_value").val().length > 0) {

                hf = hf.replace("{0}", $("#border_value").val());
                $(link).attr("href", hf);
                url = hf;
            } else {
                alert("请先选择要操作的数据行！");
                return true;
            }
        }
        return false;
    }
    var DataAuto = function () {        
        $("div[data-auto=true]").each(function (index, elem) {            
            if ($(elem).data("isload")) { return; }            
            var url = $(elem).data("auto-url");            
            LoadHtmlByAjax(url, elem, "");            
        });
    }
    function LoadHtmlByAjax(url, elem, data) { 
        $.ajax({
            url: url,
            cache: false,
            data: data,
            dataType: "html",
            type: "GET",
            success: function (data, textStatus) {
                $(elem).data("isload", true);
                $(elem).html(data);
                DataAuto();//自动加载
            },
            error: function () {
                $(elem).html("加载异常 <a class='row-edit' href='javascript:void(0);' onclick=\"ReloadHtmlData('" + url + "');\">刷新</a>");
            }
        });
    }
    window.ReloadHtmlData = function (url) {
        var elem = $("div[data-auto-url='" + url + "']");
        if (elem.length > 0) {
            LoadHtmlByAjax(url, elem, "");
        }
    }
    window.ReloadHtmlDataForEdit = function (url) {
        var elem = $("div[data-auto-url='" + url + "']");
        if (elem.length > 0) {
            LoadHtmlByAjax(url, elem, { foredit: 1 });
        }
    }
    var empurl = "/Employee/JsonAllEmp";
    var selectEmp = function (btn, retId) {
        $.getJSON(empurl, {}, function (json) {
            if (retId.indexOf('#') < 0) {
                retId = "#" + retId;
            }
            var ids = "," + $(retId).val() + ",";
            var wstr = "";
            var arrpost = new Array();
            $(json).each(function (a, b) {

                if (arrpost.indexOf(b.Post) < 0) {
                    arrpost.push(b.Post);
                }
            });
            var singleSel = $(btn).data("select-single");//单选
            arrpost.forEach(function (b, a) {
                wstr += "<dt>" + b + "</dt><dd>";
                $(json).each(function (c, d) {
                    if (d.Post == b) {
                        var w = "";
                        if (ids.indexOf("," + d.Id + ",") >= 0)
                            w = " checked='checked' ";
                        if (singleSel) {
                            //wstr += '<input type="radio" ' + w + ' value="' + d.Id + '|' + d.EmpName + '" id="emp_' + d.Id + '" name="emp_' + d.Id + '"/><label for="emp_' + d.Id + '">' + d.EmpName + '</label>';
                            wstr += '<input type="radio" ' + w + ' value="' + d.Id + '|' + d.EmpName + '" id="emp_' + d.Id + '" name="emp_single_sel"/><label for="emp_' + d.Id + '">' + d.EmpName + '</label>';
                        } else {
                            wstr += '<input type="checkbox" ' + w + ' value="' + d.Id + '|' + d.EmpName + '" id="emp_' + d.Id + '" name="emp_' + d.Id + '"/><label for="emp_' + d.Id + '">' + d.EmpName + '</label>';
                        }
                    }
                });
                wstr += "</dd>";
            });
            var n = $('<div class="modal-popup" id="openNew"><dl>' + wstr + '</dl><p><input type="button" value="确定" id="btn_emp"/></p></div>')
                   .hide()
                   .appendTo(document.body)
                   .dialog({
                       title: "选择员工",
                       modal: true,
                       resizable: true,
                       draggable: true,
                       width: 500,
                       beforeClose: function () { resetForm($(this)); }
                   }).find("#btn_emp").click(function () {                                          
                       var seleinput = $("#openNew input:checked");
                       if (seleinput.length <= 0) { alert("请选择员工"); return; }                      
                       var seleIds = "";
                       var seleNames = "";
                       $(seleinput).each(function (a, b) {
                           seleIds += $(b).val().split('|')[0] + ",";
                           seleNames += $(b).val().split('|')[1] + ",";
                       });
                       seleIds = seleIds.substr(0, seleIds.length - 1);
                       seleNames = seleNames.substr(0, seleNames.length - 1);
                       $(btn).text(seleNames);
                       $(retId).val(seleIds);
                       if (retId == "#ext_EmpIds_autosave") {//自动保存
                           var _hd_tskid = $("#hid_tskId_autosave").val();                            
                           $.post("/Project/Task/UpdateExcutorMembers", { tskId: _hd_tskid, ext_EmpIds: seleIds }, function (a) {
                               if (a.success) {
                                   // window.location.reload();
                                   alert("执行人分配成功！");
                               }
                           });
                       }
                       resetForm(n);
                   })
                   .end();
        }).error(function () {
            alert("员工信息加载失败");
        });
    }
    /*操作按钮js参数判断*/
    ltJS.RigesterDataOpen(); 
    //自动加载
    DataAuto();
    $("a[data-ajax=true]").on("click", function (evt) {

        evt.preventDefault();
        //设置.nonea阻止点击
        if ($(this).is(".nonea")) {
            return
        }
        if (setHref(this)) return;
        asyncRequest(this, {
            url: this.href,
            type: "GET",
            data: []
        });
    });
    //在新选项卡中打开 (a标签中存在data-open-new=true)
    $("a[data-open-new=true]").on("click", function (event) {
        //设置.nonea阻止点击
        if ($(this).is(".nonea")) {
            event.preventDefault();
            return;
        }
        if (setHref(this)) {
            event.preventDefault();
            return;
        }
        var href = $(this).attr("href");
        var text = $(this).data("dialog-title");
        if (text == "") { text = $(this).text(); }
        if (window.parent) {
            try {
                window.parent.window.AddAndOpenLinkToIframe(href, text);
            } catch (e) { alert(e); }
            event.preventDefault();
        }
    });
    //员工选择 data-select=true, data-select-inputid=""
    $("a[data-select=true]").on("click", function (event) {
        var href = $(this).attr("href");
        var inputid = $(this).data("select-inputid");
        selectEmp(this, inputid);
        event.preventDefault();
    });
    //展开收起
    $(".lt_open_close").on("click", function (event) {
        if (!$(this).parent().next().is("dd")) { return; }
        var d = $(this).text();
        if ($(this).is(".lt_min")) {
            $(this).parent().next().slideDown();
            $(this).removeClass("lt_min");
        } else {
            $(this).parent().next().slideUp();
            $(this).addClass("lt_min");
        }
    });
    //默认弹窗口 
    if ($("#defViewDialog").is("div")) {
       
        $("#defViewDialog").dialog({ // Create the jQuery UI dialog
            title: "信息提示",
            modal: true,
            resizable: true,
            draggable: true,
            width: 600
        }).show();
    }
    clicknum = 1;
    $(".lt_operate2").on("click", function (event) {
        clicknum = 1;
        $(".btn_exis").hide();
        $(".lt_operate2").removeClass("lt_btn_over");
        $(this).addClass("lt_btn_over");
        var dw = $(this).parent().find(".btn_exis");
        dw.show();
        
    });
    //点击取消浮动
    $(document).click(function (event) {
        if (clicknum == 1) { clicknum = 0; return; }
        if ($(".lt_operate2").length>0) {
            $(".btn_exis").hide();
            $(".lt_operate2").removeClass("lt_btn_over");
        }
    });
});
//加载后刷新
function EndReLoad(t, b) {
    var res = $.parseJSON(t.responseText);
    if (res.success || res.Success) {
        ltLoad("");
        // location = location.href;
    } else {
        if (res.Error) {
            alert(res.Error);
        } else if (res.errors) {

            alert(res.errors);
        } else {
            alert("操作出现异常！");
        }
        
        //  alert("操作失败，请联系管理员");
    }
}
//ajax数据提交
function AjaxSubmit(t) {
    //判断数据是否必填，格式是否正确
    //data-val-required="邮箱不能为空" data-val="true"
    var Mes = "";
    $(t).find("[data-val=true]").each(function () {
        if ($.trim($(this).val()) == "") {
            Mes += $(this).data("val-required") + "\n";
        } else {
            //data-val-number
            if ($(this).data("val-number")) {
                if (/^-?(?:\d+|\d{1,3}(?:,\d{3})+)(?:\.\d+)?$/.test($(this).val()) == false) {
                    Mes += $(this).data("val-number") + "\n";
                }
            }
        }
    });
    if ($.trim(Mes) != "") {
        alert(Mes);
        return false;
    }
    //******************************
    $.ajax({
        cache: false,
        type: "POST",
        url: $(t).attr("action"),
        data: $(t).serialize(),// 
        //  async: false,
        error: function (request) {
            alert("服务器错误！请重试");
        },
        success: function (data) {
            $(t).html(data);
            //            alert("操作成功！");
        }
    });
    return false;
}
/******文本输入框获取或失去焦点******/
function TextAreaFocus(t) {
    try {
        //alert($(t).data("width"));
        if ($(t).data("width") == "undefinded" || $(t).data("width") == null || $(t).data("width") == "") {
            $(t).data("width", $(t).css("width"));
        }
        if ($(t).data("height") == "undefinded" || $(t).data("height") == null || $(t).data("height") == "") {
            $(t).data("height", $(t).css("height"));
        }
        $(t).css({ position: 'absolute',width: '240px', height: '200px' });
        //$(t).css({ position: 'fixed', left:$(t).scrollLeft, top: $(t).scrollTop, width: '240px', height: '200px' });
    } catch (ex) { alert(ex); }
}
function TextAreaBlur(t) {
    $(t).css({ position: 'static', width: $(t).data("width"), height: $(t).data("height") });
}
/****固定表格第一列*****/
function FirstColumnsFixed(t) {
    if ($(t).scrollLeft() <= 0) {
        $(t).find("table tr").each(function () {
            if ($(this).find("th").length > 0) {
                if ($(this).find("th:first").css("position") == 'fixed') {
                    $(this).find("th:eq(1)").remove();
                    $(this).find("th:first").css({ position: 'static' });
                }
            }
            if ($(this).find("td").length > 0) {
                if ($(this).find("td:first").css("position") == "fixed") {
                    $(this).find("td:eq(1)").remove();
                    $(this).find("td:first").css({ position: 'static', borderLeft: 'none' });
                }
            }
        });
        return;
    }
    $(t).find("table tr").each(function () {
        if ($(this).find("th").length > 0) {
            if ($(this).find("th:first").css("position") != "fixed") {
                $(this).find("th:first").after("<th></th>");
                $(this).find("th:first").css({ position: 'fixed' });
            }
        }
        if ($(this).find("td").length > 0) {
            if ($(this).find("td:first").css("position") != "fixed") {
                var crh = $(this).find("td:first").height();
                $(this).find("td:first").after("<td></td>");
                $(this).find("td:first").css({ position: 'fixed', borderLeft: '1px solid #c4c4c4', lineHeight: crh + "px", height: crh + "px" });
            }
        }
    });
}
//通用删除
function CommonAjaxDeleteFunc(url, pams, t) {
    if (!confirm("确认删除？")) {
        return;
    }
    $.post(url, pams, function (data) {
        if (data.success) {
            $(t).parent().remove();
            alert("删除成功！");
        } else {
            if (data.errors && $.trim(data.errors) != "") {
                alert(data.errors);
            } else {
                alert("删除失败！请重试。");
            }
        }
    });
}
//通用删除
function CommonAjaxDeleteFunc2(url, pams, t) {
    if (!confirm("确认删除？")) {
        return;
    }
    $.post(url, pams, function (data) {
        if (data.success) {
            //$(t).parent().remove();
            alert("删除成功！");
            location.href = location.href;
        } else {
            if (data.errors && $.trim(data.errors) != "") {
                alert(data.errors);
            } else {
                alert("删除失败！请重试。");
            }
        }
    });
}

$.extend({ 
    addcomm: {
        refreshLeftFrame: function () { 
            if ($(document).find("#refreshLeftFrame").length > 0) {
                //刷新左侧frame                                     
                window.parent.frames["left"].window.location.href = window.parent.frames["left"].window.location.href; 
            }
        }
    }
}); 