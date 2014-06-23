$(document).ready(function () {

    var options = { distance: 10, times: 3 };
    function unSuccessLogIn() {
        var fieldPassword = $('#login-form').find('input[type=password]');
        var password = fieldPassword.val();
        if (!fieldPassword.hasClass('shaking') && password != ''){
            fieldPassword.addClass('shaking');
            fieldPassword.effect('shake', options, 250, function () {
                fieldPassword.focus();
                fieldPassword.removeClass('shaking');
            });
        }

    }

    $('form').validationEngine({ scroll: false });

    $('form#login-form').on('submit.logon', function () {
        var form = jQuery(this);
        if (!form.validationEngine('validate')) {
            unSuccessLogIn();
        } else {

         jQuery.ajax({
              url: WEB_SITE_ROOT + 'User/LogOn.json',
              type: "post",
              dataType: "json",
              data: form.serialize(),
              success:function (response) {

                if (response.Success !== true) {
                    form.find('input[type=submit]').attr('disabled', false);
                    unSuccessLogIn();
                    var text = response.data && response.data.errormessage || '';
                    if(text != '') $('div.the-password').validationEngine('showPrompt',text, 'red','topRight', false);
                } else {
                    form.off('submit.logon');
                    var role = response.data.Role.toLowerCase();
                    if (role == "admingrade" || role == "adminview" || role == "adminedit")
                        role = "admin";
                    window.location.href = WEB_SITE_ROOT + 'Home/' + role + '.aspx';
                }
            } .bind(this)
         });
            form.find('input[type=submit]').attr('disabled', true);
        }
        return false;
    });


    $('form#reset-form').on('submit.reset', function () {
        var form = jQuery(this);
        if (!form.validationEngine('validate')) { return; }
        var value = form.find('input[name=email]').val();
        form.find('input[type=submit]').attr('disabled', true);
        jQuery.getJSON(WEB_SITE_ROOT + 'User/ResetPassword.json', { email: value }, function (response, data) {
            form.find('input[type=submit]').attr('disabled', false);
        });
        return false;
    });

    $('.dev-signup-btn').click(function () {
        $('#developer-signup').submit();
        return false;
    });


    $('#developer-signup').on('submit.dev-signup', function () {
        var form = jQuery(this);
        if (!form.validationEngine('validate')) { return; }
        var options = {
            url: WEB_SITE_ROOT + 'Developer/SignUp.json',
            type: "post",
            dataType: "json",
            data: form.serialize(),
            success: function (response) {
                if (response.Success == true) {
                    form.off('submit.dev-signup');
                    window.location.href = WEB_SITE_ROOT + 'Home/Developer.aspx';
                }
                else {
                    jQuery('.sign-in-errors').html(response.data.message || '');
                }
            }.bind(this)
        };
        jQuery.ajax(options);
        return false;
    });
});