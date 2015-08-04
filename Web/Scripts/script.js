var interval;

$(function () {
    $("#btnCreateWorld").click(function () {
        $('form').submit();
    });

    $("#btnNextStep").click(function () {
        $.post("Home/NextStep", null, (function (html) {
            $("#world").html(html);
        }).bind(this));
    });

    $("#btnStart").click(function () {
        $('#btnNextStep').hide();
        $('#btnStart').hide();
        $('#btnStop').show();

        interval = setInterval(function () {
            $.post("Home/NextStep", null, (function (html) {
                $("#world").html(html);
            }).bind(this));
        }, $('#interval').val());
    });

    $("#btnStop").click(function () {
        $('#btnNextStep').show();
        $('#btnStart').show();
        $('#btnStop').hide();

        clearInterval(interval);
    });
});