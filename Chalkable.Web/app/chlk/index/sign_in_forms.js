$(function(){
	// Checking for CSS 3D transformation support
	$.support.css3d = supportsCSS3D();
	var formContainer = $('#formContainer');
	// Listening for clicks on the ribbon links
	$('.flipLink').click(function(e){
		// Flipping the forms
		formContainer.toggleClass('flipped');
		// If there is no CSS3 3D support, simply
		// hide the login form (exposing the recover one)
		if(!$.support.css3d){
			$('#login').toggle();
		}
		e.preventDefault();
	});
	// A helper function that checks for the
	// support of the 3D CSS3 transformations.
	function supportsCSS3D() {
		var props = [
			'perspectiveProperty', 'WebkitPerspective', 'MozPerspective'
		], testDom = document.createElement('a');
		for(var i=0; i<props.length; i++){
			if(props[i] in testDom.style){
				return true;
			}
		}
		return false;
	}
});


$(document).ready(function () {

    var fieldPassword = $('.field-password');
    var thePassword = $('.the-password');
    var options = { distance: 13, times: 3 };
    var optionslock = { distance: 13, times: 3 };
    var errorField  = $('.sign-in-errors');
    function unSuccessLogIn() {
        var divEl = $('div.the-password');
        if (!divEl.hasClass('shaking')){
            divEl.addClass('shaking');
            divEl.effect('shake', options, 100, function () {
                fieldPassword.focus();
                divEl.removeClass('shaking');
            });
        }
        
    }


    $('form').validationEngine({ scroll: false });

    $('form#login').on('submit.logon', function () {
        var form = jQuery(this);
        if (!form.validationEngine('validate')) {
            unSuccessLogIn();
        } else {
         var options = {
              url: WEB_SITE_ROOT + 'User/LogOn.json',
              type: "post",
              dataType: "json",
              data: form.serialize(),
              success:function (response) {
                if (response.Success !== true) {
                    unSuccessLogIn();
                    var text = response.data && response.data.errormessage || '';
                    if(text != '') $('div.the-password').validationEngine('showPrompt',text, 'red','topRight', false);
                } else {
                    form.off('submit.logon');
                    window.location.href = WEB_SITE_ROOT + 'Home/' + response.data.Role + '.aspx';
                }
            } .bind(this)
         };
         jQuery.ajax(options);
        }
        return false;
    });


    $('form#recover').on('submit.reset', function () {
        var form = jQuery(this);
        if (!form.validationEngine('validate')) { return; }
        var value = jQuery('#recovery-email').val(), container = jQuery('.signin_button_container.recovery');
        container.addClass('container-ajax-loader');
        jQuery.getJSON(WEB_SITE_ROOT + 'Account/ResetPassword.json', { email: value }, function (response, data) {
            container.removeClass('container-ajax-loader');
            var msg = (response.data) ? 'Check your email and click the link.' : 'Sorry, there is no account with that email address, or there are too many password resets in a short time.';
            addMessage({
                html: '<h2>password.</h2>' +
                    ((response.data) ? '<p></p>' : '') +
                    '<p style="text-align: center;">' + msg + '</p>' +
                    '<p></p>',
                height: 160,
                width: 300,
                buttons: [{
                    text: 'GOT IT'
                }]
            });
        });
        return false;
    });

    $('.sign_in_button').click(function () {
        $('#login').submit();
        return false;
    });

    $('.email_me_button').click(function () {
        $('#recover').submit();
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