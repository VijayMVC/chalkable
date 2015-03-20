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

    function showFormBanner($form, text, exlusive) {
        exlusive && $form.find('p:not(.banner)').addClass('hide');
        $form.find('p.banner').html(text).removeClass('hide');
    }

    function hideFormBanner($form) {
        $form.find('p:not(.banner)').removeClass('hide');
        $form.find('p.banner').addClass('hide');
    }

    $('.loginContainer').on('click', 'span.close', function (event) {
        hideFormBanner($(this).parent().parent().find('form'));
    });

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

        hideFormBanner(form);

        var handler = function (responseObj) {
            var response = responseObj.responseJSON || responseObj;
            if (typeof window['mixpanel'] !== "undefined") {
                mixpanel.track(
                    "Logged in from com",
                    response || {}
                );
            }

            var hideLoader = true;
            if (!response || !response.data) {
                showFormBanner(form, 'Our server is not responding. Please try again soon', false);
            } else if (response.success == true) {
                form.off('submit.logon');
                var role = (response.data.role || response.data.Role).toLowerCase();
                if (role == "admingrade" || role == "adminview" || role == "adminedit")
                    role = "admin";
                window.location.href = WEB_SITE_ROOT + 'Home/' + role + '.aspx';
                hideLoader = false;
            } else if ((response.data.exceptiontype || response.data.Exceptiontype) == 'ChalkableSisNotFoundException') {
                showFormBanner(form, 'Your InformationNow server is not responding. Please try again soon', false);
            } else {
                var text = response.data.message || '';
                if (text != '')
                    $('#email').validationEngine('showPrompt', text, 'red', 'topRight', false);
                else
                    unSuccessLogIn();
            }

            hideLoader && form.find('[type=submit]')
                .removeAttr('disabled')
                .removeClass('working');
        }.bind(this);

        jQuery.ajax({
            url: WEB_SITE_ROOT + 'User/LogOn.json',
            type: "post",
            dataType: "json",
            data: form.serialize(),
            success: handler,
            error: handler
        });

        return false;
    });


    $('form#reset-form').on('submit.reset', function () {
        var form = jQuery(this);
        if (!form.validationEngine('validate')) {
            return false;
        }

        var handler = function (responseObj) {
            var response = responseObj.responseJSON || responseObj;

            if (!response || !response.data) {
                showFormBanner(form, 'Our Server is not responding. Please try again soon', false);
            } else if (response.data.exceptiontype == 'ChalkableException') {
                showFormBanner(form, response.data.message, true);
            } else {
                showFormBanner(form, 'Please check your inbox, we sent you password recovery email.', true);
            }

            form.find('[type=submit]')
                .removeAttr('disabled')
                .removeClass('working');
        }.bind(this);

        var value = form.find('input[name=email]').val();
        jQuery.ajax({
            url: WEB_SITE_ROOT + 'User/ResetPassword.json',
            type: "post",
            dataType: "json",
            data: {email: value},
            success: handler,
            error: handler
        });

        hideFormBanner(form);
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

        var handler = function (response) {
            form.find('[type=submit]')
                .removeAttr('disabled')
                .removeClass('working');

            var R = response.responseJSON || response;
            if (!R || !R.data) {
                alert('Our Server is not responding. Please try again soon');
            } else if (R.success == true) {
                var role = R.data.role.toLowerCase();
                window.location.href = WEB_SITE_ROOT + 'Home/' + role + '.aspx';
            } else {
                var text = R.data.message || '';
                (text != '') && $('#email2').validationEngine('showPrompt', text, 'red', 'topRight', false);
            }
        }.bind(this);

        jQuery.ajax({
            url: WEB_SITE_ROOT + 'Developer/SignUp.json',
            type: "post",
            dataType: "json",
            data: form.serialize(),
            success: handler,
            error: handler
        });

        form.find('[type=submit]')
            .attr('disabled', true)
            .addClass('working');

        return false;
    });
});