$(document).ready(function() {
	$("#filetable").DataTable({
		"order": [[2, "desc"]],
		"language": {
			"url": "//cdn.datatables.net/plug-ins/1.10.10/i18n/Czech.json"
		}
	});
});