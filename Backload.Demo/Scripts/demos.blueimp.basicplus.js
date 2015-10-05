/*jslint unparam: true */
/*global window, $ */
$(function () {
    'use strict';

    // We use the upload handler integrated into Backload:
    // In this example we set an objectContect (id) in the url query (or as form parameter).
    // You can use a user id as objectContext give users only access to their own uploads.
    var url = '/Backload/FileHandler?objectContext=C5F260DD3787';

    $('#fileupload').fileupload({
        url: url,
        dataType: 'json',
        autoUpload: false,
        disableImageResize: /Android(?!.*Chrome)|Opera/
            .test(window.navigator && navigator.userAgent),
        previewCrop: true,
        maxChunkSize: 10000000                                          // Optional: file chunking with 10MB chunks
    })
    .bind('fileuploadsubmit', function (e, data) {
        // Optional: We add a random uuid form parameter. On chunk uploads the uuid is used to store the chunks.
        data.formData = { uuid: Math.random().toString(36).substr(2, 8) };
    })

        // if the following init method call causes problems bind event handlers manually 
        // like in blueimps basic plus example (https://blueimp.github.io/jQuery-File-Upload/basic-plus.html)
    .data('blueimp-fileupload').initTheme("BasicPlus");
});
