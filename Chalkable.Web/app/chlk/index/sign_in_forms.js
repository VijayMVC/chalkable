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
            return false;
        }

        form.find('[type=submit]')
            .attr('disabled', true)
            .addClass('working');

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

                form.find('[type=submit]')
                    .removeAttr('disabled')
                    .removeClass('working');
            }.bind(this),

            error: function (response){
                if(!response.responseJSON){
                    alert('Our Server is not responding. Please try again soon');
                } else if(response.responseJSON.exceptiontype == 'ChalkableSisNotFoundException'){
                    alert('Your iNow Server is not responding. Please try again soon');
                }

                form.find('[type=submit]')
                    .removeAttr('disabled')
                    .removeClass('working');
            }.bind(this)
        });

        return false;
    });


    $('form#reset-form').on('submit.reset', function () {
        var form = jQuery(this);
        if (!form.validationEngine('validate')) {
            return false;
        }

        var value = form.find('input[name=email]').val();
        jQuery.ajax({
            url: WEB_SITE_ROOT + 'User/ResetPassword.json',
            type: "post",
            dataType: "json",
            data: {email: value},
            success: function (response, data) {
                form.find('[type=submit]')
                    .removeAttr('disabled')
                    .removeClass('working');
            },

            error: function (response) {
                if (!response.responseJSON) {
                    alert('Our Server is not responding. Please try again soon');
                } else if (response.responseJSON.exceptiontype == 'ChalkableException') {
                    alert(response.responseJSON.message);
                }

                form.find('[type=submit]')
                    .removeAttr('disabled')
                    .removeClass('working');
            }.bind(this)
        });

        form.find('[type=submit]')
            .attr('disabled', true)
            .addClass('working');

        return false;
    });

    $('.dev-signup-btn').click(function () {
        $('#developer-signup').submit();
        return false;
    });


    $('#sign-up-form').on('submit.dev-signup', function () {
        var form = jQuery(this);
        if (!form.validationEngine('validate')) {
            return false;
        }

        jQuery.ajax({
            url: WEB_SITE_ROOT + 'Developer/SignUp.json',
            type: "post",
            dataType: "json",
            data: form.serialize(),
            success: function (response) {
                form.find('[type=submit]')
                    .removeAttr('disabled')
                    .removeClass('working');
                if (response.Success == true) {
                    var role = response.data.Role.toLowerCase();
                    window.location.href = WEB_SITE_ROOT + 'Home/' + role + '.aspx';
                }
                else {
                    var text = response.data && response.data.message || '';
                    if(text != '') $('#email2').validationEngine('showPrompt',text, 'red','topRight', false);
                }
            }.bind(this),

            error: function (response) {
                if (!response.responseJSON) {
                    alert('Our Server is not responding. Please try again soon');
                } else if (response.responseJSON.exceptiontype == 'ChalkableSisNotFoundException') {
                    alert('Your iNow Server is not responding. Please try again soon');
                }

                form.find('[type=submit]')
                    .removeAttr('disabled')
                    .removeClass('working');
            }.bind(this)
        });

        form.find('[type=submit]')
            .attr('disabled', true)
            .addClass('working');

        return false;
    });
});