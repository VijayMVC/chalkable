$(document).ready(function(){
	var $loginButton=$('.mainNav a.lg'),
		$loginPanel=$('#login'),
		$closelogin=$loginPanel.find('span.close'),
		$mobileMenuIcon=$('.wrap a.listMenu'),
		$menuNav=$('.wrap nav'),
		$feature=$menuNav.find('a[data-type=feature]'),
		$resetPasswordAnc=$('#makeLogin .forgotPassword a'),
		$makeLogin=$('#makeLogin'),
		$BackToLogInAnc=$('#resetPassword .forgotPassword a'),
		$resetPassword=$('#resetPassword');
	
	//open login page after a click event on the login button
	$loginButton.on('click', function(e){
		e.preventDefault();
		if($(window).width()>1024){
			$loginPanel.fadeIn();
		}else{
			$loginPanel.fadeIn(100);
			if($mobileMenuIcon.is(':visible')){
				$menuNav.removeClass('openNavigation');
				$mobileMenuIcon.removeClass('clicked');	
			}
		}	
	});

	//close login page after a click event on the close button
	$closelogin.on('click', function(){
		closePopUp();
	});

	//close login page if you click somewhere outside the login panel
	$loginPanel.on('click', function(event){
		if($(event.target).is('div#login')){
			closePopUp();
		}
	});

	function closePopUp(){
		if($(window).width()>1024){
			$loginPanel.fadeOut(function(){
				$resetPassword.addClass('hide');
    			$makeLogin.removeClass('hide');
			});
		}else{	
			$loginPanel.fadeOut(100, function(){
				$resetPassword.addClass('hide');
    			$makeLogin.removeClass('hide');
			});
		}
	}

	//mobile menu behaviour 
	$mobileMenuIcon.on('click', function(e){
		e.preventDefault();
		$menuNav.toggleClass('openNavigation');
		$mobileMenuIcon.toggleClass('clicked');	
	});

	//smooth scrolling to content
	$feature.on('click', function(e){
		e.preventDefault();
		var $target= $(this.hash);
            // $target= $(target);
        $('body,html').animate(//html-to fix a bug with firefox
            {'scrollTop':$target.offset().top},
            900,'swing'
        );
        if($mobileMenuIcon.is(':visible')){
				$menuNav.removeClass('openNavigation');
				$mobileMenuIcon.removeClass('clicked');	
		}
    });

	//switch to reset password panel if click on the reset password link
    $resetPasswordAnc.on('click', function(e){
    	e.preventDefault();
    	$makeLogin.addClass('hide');
    	$resetPassword.removeClass('hide');
    });

	//switch to login panel if click on the rback to login link
    $BackToLogInAnc.on('click', function(e){
    	e.preventDefault();
    	$resetPassword.addClass('hide');
    	$makeLogin.removeClass('hide');
    });
});

//Add Message

function addMessage(o){

    function close(){
        jQuery('body').find('.modal-bg').remove();
        jQuery('body').find('.pop-message').remove();
        o.onClose && o.onClose();
    }

    var msgAlreadyShown = jQuery('body').find('.modal-bg') != undefined && jQuery('body').find('.pop-message') != undefined;

    if (msgAlreadyShown && !o.tourMessage)
    {
        close();
    }



    jQuery('body').append((!o.notModal ? '<div class="modal-bg ' + (o.modalCls || '') + '" style="' + (o.zIndex ? ' z-index:' + o.zIndex + ';' : '' ) + '">' : '') + '</div>' +
        '<div class = "pop-message ' + (o.cls || '') + '" style="' +
        (o.height ? ' height:' + o.height + 'px;' : '' ) +
        (o.width ? ' width:' + o.width + 'px;' : '' ) +
        (o.left ? ' left:' + o.left + 'px;' : '' ) +
        (o.top ? ' top:' + o.top + 'px;' : '' ) +
        (o.zIndex ? ' z-index:' + o.zIndex + ';' : '' ) +
        (o.absolute ? 'position:absolute;' : (o.fixed ? 'position:fixed;' :'')) + '">'  + (o.newStyle ? '<div class="pop-wrapper">' : '')+
        (o.close ? '<div class="pop-close"></div>' : '') +
        (o.html || ( o.text ?  ('<p class="text">' + o.text + '</p>') : '')) +
        ('<div id="buttons-container" class="' + ((o.containerCls) ? o.containerCls : '') + '"><button class="msg-button">OK</button></div>') + (o.newStyle ? '</div>' : '')+
        (o.tip ? '<div class="tip">' + o.tip + '</div>' : '') +
        '</div>');

    o.afterLoad && o.afterLoad();

    jQuery('body').on('click', '.msg-button', close);

    if(o.newStyle){
        var pop = jQuery('.pop-message');
        var div = pop.find('.pop-wrapper');
        pop.css({
            height: div.height(),
            width: div.width()
        });
    }

    if(o.closeOnModalClick){
        jQuery('body').find('.modal-bg').on('click', close);
    }

    jQuery('.pop-close').click(close);

    return {
        close: close
    };
}