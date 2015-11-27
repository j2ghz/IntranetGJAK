﻿/// <reference path="tsd.d.ts" />

/*
 * jQuery File Upload Plugin JS Example
 * https://github.com/blueimp/jQuery-File-Upload
 *
 * Copyright 2010, Sebastian Tschan
 * https://blueimp.net
 *
 * Licensed under the MIT license:
 * http://www.opensource.org/licenses/MI
3T
 */

/* global $, window */

$(() => {
    "use strict";
    var uploadUrl = "Files/Index";
    // Initialize the jQuery File Upload widget:
    $("#fileupload").fileupload({
        // Uncomment the following to send cross-domain cookies:
        //xhrFields: {withCredentials: true},
        url: uploadUrl
    });

    //// Enable iframe cross-domain access via redirect option:
    //$('#fileupload').fileupload(
    //    'option',
    //    'redirect',
    //    window.location.href.replace(
    //        /\/[^\/]*$/,
    //        '/cors/result.html?%s'
    //    )
    //);

    //if (window.location.hostname === 'blueimp.github.io') {
    //    // Demo settings:
    //    $('#fileupload').fileupload('option', {
    //        url: '//jquery-file-upload.appspot.com/',
    //        // Enable image resizing, except for Android and Opera,
    //        // which actually support image resizing, but fail to
    //        // send Blob objects via XHR requests:
    //        disableImageResize: /Android(?!.*Chrome)|Opera/
    //            .test(window.navigator.userAgent),
    //        maxFileSize: 999000,
    //        acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i
    //    });
    //    // Upload server status check for browsers with CORS support:
    //    if ($.support.cors) {
    //        $.ajax({
    //            url: '//jquery-file-upload.appspot.com/',
    //            type: 'HEAD'
    //        }).fail(function () {
    //            $('<div class="alert alert-danger"/>')
    //                .text('Upload server currently unavailable - ' +
    //                        new Date())
    //                .appendTo('#fileupload');
    //        });
    //    }
    //} else {
    // Load existing files:
    $("#fileupload").addClass("fileupload-processing");
    var settings: JQueryAjaxSettings= {
        url: uploadUrl,
        dataType: "json",
        context: $("#fileupload")[0]
    };
    $.ajax(settings).always(function () {
        $(this).removeClass("fileupload-processing");
    }).done(function (result) {
        var fu: any = $(this).fileupload("option", "done");
            fu.call(this, $.Event("done"), { result: result });
    });
});