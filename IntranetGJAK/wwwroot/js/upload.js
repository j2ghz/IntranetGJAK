/* global $, window */
$(() => {
	"use strict";
	var uploadUrl = "/api/files";
	// Initialize the jQuery File Upload widget:
	$("#fileupload").fileupload({
		url: uploadUrl
	});

	$("#fileupload").addClass("fileupload-processing");
	var settings = {
		url: uploadUrl,
		dataType: "json",
		context: $("#fileupload")[0]
	};
	$.ajax(settings).always(function() {
		$(this).removeClass("fileupload-processing");
	}).done(function(result) {
		$(this).fileupload("option", "done").call(this, $.Event("done"), { result: result });
	});
});