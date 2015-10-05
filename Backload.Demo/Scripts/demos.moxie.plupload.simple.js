$(function () {
    // We use the upload handler integrated into Backload:
    var url = '/Backload/FileHandler';

    var uploader = new plupload.Uploader({
        runtimes: 'html5,html4',
        browse_button: 'pickfiles', // you can pass in id...
        container: document.getElementById('container'), // ... or DOM Element itself
        url: url,

        filters: {
            max_file_size: '10mb',
            mime_types: [
                { title: "Image files", extensions: "jpg,gif,png,pdf" }
            ]
        },

        init: {
            PostInit: function () {
                document.getElementById('filelist').innerHTML = '';

                document.getElementById('uploadfiles').onclick = function () {
                    uploader.start();
                    return false;
                };
            },

            FilesAdded: function (up, files) {
                plupload.each(files, function (file) {
                    document.getElementById('filelist').innerHTML += '<div id="' + file.id + '">' + file.name + ' (' + plupload.formatSize(file.size) + ') <b></b></div>';
                });
            },

            UploadProgress: function (up, file) {
                document.getElementById(file.id).getElementsByTagName('b')[0].innerHTML = '<span>' + file.percent + "%</span>";
            },

            Error: function (up, err) {
                document.getElementById('console').innerHTML += "\nError #" + err.code + ": " + err.message;
            }
        },

        // In this example we set an objectContect (id) as a form parameter (query parameter also allowed).
        // You can use a user id as objectContext give users only access to their own uploads.
        // You need to set the plugin parameter only if you do not set it in the server side config (default: JQueryFileUpload)
        multipart_params: {
            "plugin": "PlUpload",
            "objectContext": "3E8A03244079"
        }

    });

    uploader.init();
});