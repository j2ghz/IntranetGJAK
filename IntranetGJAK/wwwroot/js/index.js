
        $(document).ready(function () {
            $("#filetable").DataTable({
                "order": [[2, "desc"]],
                "language": {
                    "sEmptyTable": "Tabulka neobsahuje žádná data",
                    "sInfo": "Zobrazuji _START_ až _END_ z celkem _TOTAL_ záznamù",
                    "sInfoEmpty": "Zobrazuji 0 až 0 z 0 záznamù",
                    "sInfoFiltered": "(filtrováno z celkem _MAX_ záznamù)",
                    "sInfoPostFix": "",
                    "sInfoThousands": " ",
                    "sLengthMenu": "Zobraz záznamù _MENU_",
                    "sLoadingRecords": "Naèítám...",
                    "sProcessing": "Provádím...",
                    "sSearch": "Hledat:",
                    "sZeroRecords": "Žádné záznamy nebyly nalezeny",
                    "oPaginate": {
                        "sFirst": "První",
                        "sLast": "Poslední",
                        "sNext": "Další",
                        "sPrevious": "Pøedchozí"
                    },
                    "oAria": {
                        "sSortAscending": ": aktivujte pro øazení sloupce vzestupnì",
                        "sSortDescending": ": aktivujte pro øazení sloupce sestupnì"
                    }
                }
            });
        });