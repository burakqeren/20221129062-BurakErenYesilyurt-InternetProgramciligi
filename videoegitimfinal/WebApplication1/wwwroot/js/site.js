$(document).ready(function () {

    egitimkategoriAdıfind();

    function egitimkategoriAdıfind() {
        $("#egitimtable tbody").empty();
        $.ajax({
            url: "/Admin/EgitimGetir",
            type: "Get",
            data: {},
            success: function (data) {
                var i = 1;
                $.each(data, function (index, item) {
                    var tr = $('<tr id="' + item.id + '"></tr>');
                    tr.append('<td>' + i + '</td>');
                    tr.append('<td>' + item.kategoriAdı + '</td>');

                    var btnGuncelle = $('<button type="button" id="duzenleButton" class="btn btn-secondary">Düzenle</button>');
                    btnGuncelle.click(function () {
                        btnGuncelleClick(item.id, item.kategoriAdı);
                    });
                    var td = $('<td></td>');
                    td.append(btnGuncelle);
                    tr.append(td);

                    $("#egitimtable tbody").append(tr);
                    i++;
                });
            }
        });
    }

    function btnGuncelleClick(id, kategoriAdı) {
        $("#id").val(id);
        $("#kategoriAdı").val(kategoriAdı);
    }





    $("button.btn-success").click(function () {
        event.preventDefault();

        var id = $("#id").val();
        var kategoriAdı = $("#kategoriAdı").val();

        if (kategoriAdı.trim() === "") {

            toastr.error('Hatalı İşlem yapıyorsunuz.');
            return;
        }
        if (id.trim() === "") {

            toastr.error('Hatalı İşlem yapıyorsunuz.');
            return;
        }




        $.ajax({
            url: "/Admin/EgitimGuncelle",
            type: "POST",
            data: { id: id, kategoriAdı: kategoriAdı },
            success: function (response) {
                toastr.success('Eğitim Adı Başarılı Bir Şekilde Güncelleştirildi.');
                $("#id").val("");
                $("#kategoriAdı").val("");
                egitimkategoriAdıfind();
            },
            error: function () {
                console.error("Veri güncelleme işleminde hata oluştu.");
            }
        });
    });

    $("button.btn-danger").click(function () {
        event.preventDefault();

        var id = $("#id").val();
        var kategoriAdı = $("#kategoriAdı").val();
        if (kategoriAdı.trim() === "") {
            toastr.error('Hatalı İşlem yapıyorsunuz.');
            return;
        }

        $.ajax({
            url: "/Admin/EgitimSil",
            type: "POST",
            data: { id: id, kategoriAdı: kategoriAdı },
            success: function (response) {
                toastr.success('Başarılı Bir Şekilde Silindi.');
                $("#id").val("");
                $("#kategoriAdı").val("");
                egitimkategoriAdıfind();
            },
            error: function () {
                console.error("Veri Silme işleminde hata oluştu.");
            }
        });
    });
    $("button.btn-info").click(function () {
        event.preventDefault();

        var id = $("#id").val();
        var kategoriAdı = $("#kategoriAdı").val();


        if (kategoriAdı.trim() === "") {

            toastr.error('Hatalı İşlem yapıyorsunuz.');
            return;
        }

        $.ajax({
            url: "/Admin/EgitimEkle",
            type: "POST",
            data: { kategoriAdı: kategoriAdı },
            success: function (response) {
                toastr.success('Başarılı Bir Şekilde Silindi.');
                $("#id").val("");
                $("#kategoriAdı").val("");
                egitimkategoriAdıfind();
            },
            error: function () {
                console.error("Veri kaydetme işleminde hata oluştu.");
            }
        });
    });
});

