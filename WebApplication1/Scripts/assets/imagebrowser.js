var args = parent.tinymce.activeEditor.windowManager.getParams();
var winJ = args['jquery'];
var win = args['window'];
var field = '#' + args['input'];
var input = winJ(field);


$(document).ready(function () {


  $('#fileUpload').on('change', function () {
    if ($(this).val()) {
      var form = $(this.form);
      $.ajax(form.prop('action'), {
        data: form.find('input[type=hidden]').serializeArray(),
        dataType: 'json',
        files: form.find(':file'),
        iframe: true,
        processData: false
      }).always(function () {
        console.log('process finished');
      }).done(function (data) {
        window.location.reload();
      });
    }
    $("#ErrorMessage").text("");
  });

  $('.file-list-wrapper li').click(function () {
    var count = $('.file-list-wrapper li.selected').length;

    if (count == 1 && !$(this).hasClass('selected') ) {
      $('.file-list-wrapper li.selected').removeClass('selected');
    }

    $(this).toggleClass('selected');
    $("#ErrorMessage").text("");
  });

  $('#insertBtn').click(function () {
      var imageURL = window.location.hostname + "/Media/File/";
      console.log(imageURL);
    if ($('.file-list-wrapper li.selected').length == 1) {
      var thumbUrl = $('.file-list-wrapper li.selected img').attr('src');
      var param = thumbUrl.substring(thumbUrl.indexOf('?'));
      param = param.split('&')[0].split("=")[1];
      input.val(imageURL + param);
      win.tinyMCE.activeEditor.windowManager.close(window);
      $("#ErrorMessage").text("");
    }
    else {
        $("#ErrorMessage").text("Please select Image to Insert.");
    }
  });

    
  $(document).keydown(function (e) {
      // ESCAPE key pressed
      if (e.keyCode == 27) {
          win.tinyMCE.activeEditor.windowManager.close(window);
      }
  });

  $('#cancelBtn').click(function () {
    win.tinyMCE.activeEditor.windowManager.close(window);
  });

  $('#deleteBtn').click(function () {
      if ($('.file-list-wrapper li.selected').length == 1) {
          var thumbUrl = $('.file-list-wrapper li.selected img').attr('src');
          var param = thumbUrl.substring(thumbUrl.indexOf('?')+1);
          $.ajax({
              type: "POST",
              url: "Delete",
              data: param,
              dataType: "json",
              iframe: true
          }).done(function (msg) {
              window.location.reload();
          });
          $("#ErrorMessage").text("");
      }
      else {
          $("#ErrorMessage").text("Please select Image to Delete.");
      }
  });
});