$(document).ready(function () {

    $('form').validationEngine({ scroll: false });

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
