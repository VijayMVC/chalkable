$(document).ready(function () {

    var options = { distance: 10, times: 3 };
    function unSuccessLogIn() {
        var fieldPassword = $('#login-form').find('input[type=password]');
        var password = fieldPassword.val();
        if (!fieldPassword.hasClass('shaking') && password != ''){
            fieldPassword.addClass('shaking');
            setTimeout(function () {
                fieldPassword.focus();
                fieldPassword.removeClass('shaking');
            }, 500);
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
              success: function (response) {

                if (typeof window['mixpanel'] !== "undefined") {
                    mixpanel.track(
                        "Logged in from com",
                        response.data || {}
                    );
                }

                if (response.Success !== true) {
                    form.find('input[type=submit]').attr('disabled', false);
                    //todo : think how to write this better
                    var text = response.data && response.data.errormessage || response.ErrorMessage || '';
                    if(text != '')
                        $('#email').validationEngine('showPrompt',text, 'red','topRight', false);
                    else
                        unSuccessLogIn();
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


    $('#sign-up-form').on('submit.dev-signup', function () {
        var form = jQuery(this);
        if (!form.validationEngine('validate')) { return; }
        var options = {
            url: WEB_SITE_ROOT + 'Developer/SignUp.json',
            type: "post",
            dataType: "json",
            data: form.serialize(),
            success: function (response) {
                form.find('input[type=submit]').prop('disabled', false);
                if (response.Success == true) {
                    var role = response.data.Role.toLowerCase();
                    window.location.href = WEB_SITE_ROOT + 'Home/' + role + '.aspx';
                }
                else {
                    var text = response.data && response.data.message || '';
                    if(text != '') $('#email2').validationEngine('showPrompt',text, 'red','topRight', false);
                }
            }.bind(this)
        };
        jQuery.ajax(options);
        form.find('input[type=submit]').prop('disabled', true);
        return false;
    });
});