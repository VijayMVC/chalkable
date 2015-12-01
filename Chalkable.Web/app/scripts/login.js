/**
 * Created by Volodymyr on 9/11/2015.
 */

+function ($, location) {
    $('.login-page .body A').on('click', function (event) {
        var $this = $(this);

        var $target = $($this.attr('href') + '-form');
        if ($target.get(0)) {
            $target
                .slideDown(300)
                .siblings('.modal-form')
                    .slideUp(100);

        }
    });

    $(function () {
        var $target = $(location.hash + '-form');
        if ($target.get(0)) {
            $target
                .slideDown(300)
                .siblings('.modal-form')
                .slideUp(100);

        }
    });
}(jQuery, window.location);

+function ($, WEB_SITE_ROOT, mixpanel, location) {
    function showFormBanner($form, msg) {
        $form.find('.banner').html(msg).show();
    }

    function hideFormBanner($form) {
        $form.find('.banner').text('').hide();
    }

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

    var validateForm = $('#login-form form')
        .on('submit', function () {
            var $form = $(this);

            if (!validateForm.checkForm())
                return false;

            hideFormBanner($form);

            $form.find('[type=submit]')
                .attr('disabled', true)
                .addClass('working');

            var handler = function (responseObj) {
                var response = responseObj.responseJSON || responseObj;
                mixpanel && mixpanel.track("Logged in from login page", response || {});

                var hideLoader = true;
                if (!response || !response.data) {
                    showFormBanner($form, 'Our server is not responding. Please try again soon', false);

                } else if (response.success === true) {
                    $form.off('submit');
                    var role = (response.data.role || response.data.Role).toLowerCase();
                    if (role == "admingrade" || role == "adminview" || role == "adminedit")
                        role = "admin";
                    location.href = WEB_SITE_ROOT + 'Home/' + role + '.aspx';
                    hideLoader = false;

                } else if ((response.data.exceptiontype || response.data.Exceptiontype) === 'ChalkableSisNotFoundException') {
                    showFormBanner($form, 'Your InformationNow server is not responding. Please try again soon', false);

                } else {
                    var text = response.data.message || '';
                    if (text !== '')
                        validateForm.showErrors({UserName: text});
                    else
                        unSuccessLogIn();
                }

                hideLoader && $form.find('[type=submit]')
                    .removeAttr('disabled')
                    .removeClass('working');
            };

            $.ajax({
                url: WEB_SITE_ROOT + 'User/LogOn.json',
                type: "post",
                dataType: "json",
                data: $form.serialize(),
                success: handler,
                error: handler
            });

            return false;
        })
        .validate({
        });

}(jQuery, window.WEB_SITE_ROOT, window.mixpanel, window.location);

+function ($, WEB_SITE_ROOT, mixpanel, location) {
    function showFormBanner($form, msg, infoMsg) {
        $form.find('.banner').html(msg).toggleClass('info', infoMsg).show();
    }

    function hideFormBanner($form) {
        $form.find('.banner').hide().text('').removeClass('info');
    }

    var validateForm = $('#forgot-password-form form')
        .on('submit', function () {
            var $form = $(this);

            if (!validateForm.checkForm())
                return false;

            hideFormBanner($form);

            $form.find('[type=submit]')
                .attr('disabled', true)
                .addClass('working');

            var handler = function (responseObj) {
                var response = responseObj.responseJSON || responseObj;

                if (!response || response.data === null) {
                    showFormBanner($form, 'Our Server is not responding. Please try again soon', false);
                } else if (response.success === true && response.data === false) {
                    validateForm.showErrors({UserName: 'Email is not associated with any account'});
                } else if (response.data.exceptiontype == 'ChalkableException') {
                    showFormBanner($form, response.data.message, true);
                } else {
                    showFormBanner($form, 'We just sent you a password recovery email. Please check your inbox.', true);
                    $form.find('.form-group').hide();
                }

                $form.find('[type=submit]')
                    .removeAttr('disabled')
                    .removeClass('working');

            };

            var value = $form.find('input[name=UserName]').val();
            jQuery.ajax({
                url: WEB_SITE_ROOT + 'User/ResetPassword.json',
                type: "post",
                dataType: "json",
                data: {email: value},
                success: handler,
                error: handler
            });

            return false;
        })
        .validate({
        });

}(jQuery, window.WEB_SITE_ROOT, window.mixpanel, window.location);
