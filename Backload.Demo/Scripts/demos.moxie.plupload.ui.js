$(function () {
    // In this example we set an objectContect (id) as a url query parameter (form parameter also allowed, but in GET requests only query)
    // You can use a user id as objectContext give users only access to their own uploads.
    // You need to set the plugin parameter only if you do not set it in the server side config (default: JQueryFileUpload)
    var url = "/Backload/FileHandler?plugin=PlUpload&objectContext=3E8A03244079";

    $("#uploader").plupload({
        // General settings
        runtimes: 'html5,html4',
        url: url,
        max_file_size: '100mb',
        chunk_size: '10mb',
        filters: [{ title: "Image files", extensions: "jpg,gif,png,pdf" } ],
        rename: true,
        sortable: true,
        dragdrop: true,
        views: { list: true, thumbs: true, active: 'thumbs' }
    });
    var uploader = $('#uploader').plupload('getUploader');


    // PlUpload doesn't send a delete request to the server automatically, so we do it with an jquery ajax request
    uploader.bind("FilesRemoved", function (up, files) {
        $.each(files, function (i, file) {
            if ((typeof file.deleteUrl !== "undefined") && (file.deleteUrl != "")) {
                $.ajax({
                    url: file.deleteUrl, 
                    type: "DELETE",
                    dataType: "json"
                }).done(function (data, status, jqXHR) {
                    if (status != true) {
                        // Add error handling.
                    }
                });
            }
        });
    });


    // After a file was uploaded we extend the internal file class in the plupload.files array with a delete url.
    uploader.bind('FileUploaded', function (up, files, result) {
        var remoteFiles = JSON.parse(result.response).files;
        if (!$.isArray(files)) files = [files];
        $.each(files, function (i, file) {
            file.deleteUrl = remoteFiles[i].deleteUrl;
            file.fileUrl = remoteFiles[i].fileUrl;
            file.thumbnail = remoteFiles[i].thumbnail;
        });
    });


    // We do not use the file added event but if you need to manipulate the dom, give PlUpload a little time to add the files to the list
    uploader.bind("FilesAdded", function (up, files) {
        setTimeout(function () {
            var $list = $('ul#uploader_filelist');
            $.each(files, function (i, file) {
                if (file.existing)
                {
                    var $thumb = $list.find('li#' + file.id + ' div.plupload_file_dummy').first();
                    if ($thumb.length == 1)
                    {
                        $thumb.empty()
                            .removeClass('plupload_file_dummy')
                            .css('overflow', 'hidden')
                            // Add the thumbnail. You can avoid setting the width, if you define the thumbnail-size in the Backload configuration
                            .html('<img src="' + file.thumbnail + '" title="' + file.name + '" style="width:100px" />'); 
                    }
                }
            });
        }, 50);
    });


    // Load existing files, if any.
    $.ajax({
        url: url,
        type: "GET",
        dataType: "json"
    }).done(function (data, textStatus, jqXHR) {
        var upFiles = data.files;
        var files = [];
        for (var i = 0; i < upFiles.length; i++) {
            var file = new plupload.File("", upFiles[i].name, upFiles[i].size);
            file.percent = upFiles[i].percent + "%";
            file.name = upFiles[i].name;
            file.loaded = upFiles[i].size;
            file.size = upFiles[i].size;
            file.origSize = upFiles[i].size;
            file.status = plupload.DONE;
            file.type = upFiles[i].type;
            file.thumbnail = upFiles[i].thumbnail;
            file.fileUrl = upFiles[i].fileUrl;
            file.deleteUrl = upFiles[i].deleteUrl;
            file.existing = true;                       // custom flag do identify existing files
            files.push(file);
        }
        if (uploader) uploader.addFile(files);
    });
});