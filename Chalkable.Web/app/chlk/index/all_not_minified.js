//All homepage misc. scripts
$(window).load(function () {
    $('.cycle').hide().fadeIn(500);
    try{
        $('.cycle').cycle({
            //fx: 'toss',// choose your transition type, ex: fade, scrollUp, shuffle, etc...
            fx: 'turnDown',
            animOut: {
                bottom:  140
            },
            speedIn:  300,
            speedOut: 300,
            speed:  150,
            delay: 0,
            timeout:1500
        });
    }catch(e){

    }

    $('.signupbutton').click(function(){
        $('form').submit();
        return false;
    });
    $('input[name=availheight]').val(window.screen.availHeight);
    $('input[name=availwidth]').val(window.screen.availWidth);
    $('form').validationEngine && $('form').validationEngine({ scroll: false });
});

jQuery(document).ready(function() {

    // Expand Sign in Panel
    $("#open").click(function(){
        $("div#panel").slideToggle(500, 'swing');
        $("button.open").hide();
        $(".michaels_border").addClass("downstate");
    });

    // Collapse Panel
        $("#close").click(function(){
        $("div#panel").slideToggle(500, 'swing');
        $("button.open").delay(200).fadeIn();
        $(".michaels_border").removeClass("downstate");
    });

    $("a.utube").hover(function() {
        $(".vidyard_play_button").toggleClass("hovered");
    });


//navbar sticky and highlight

    google.setOnLoadCallback(function() {
        $(window).scroll(sticky_relocate);
        sticky_relocate();
    });
    function sticky_relocate() {
        try{
            var window_top = $(window).scrollTop();
            var div_top = $('#sticky-anchor').offset().top-1;
            var pricing_top = $('#sticky-anchor-pricing').offset().top;

            if (window_top > div_top){
                $('.demo_container').addClass('stick');
                $('.demo_container').addClass('stick2');
                $('a#nav_section1').addClass('stick');
                $('a#nav_section_two').removeClass('stick');
            }
            else{
                $('.demo_container').removeClass('stick');
                    $('a#nav_section1').removeClass('stick');
                    $('.demo_container').removeClass('stick2');
            }
            if (window_top >= pricing_top) {
              $('a#nav_section1').removeClass('stick');
                $('a#nav_section_two').addClass('stick');
            }
        }catch(e){

        }

    }



//smooth scroll
    $(".scroll").click(function(event){
        event.preventDefault();
        $('html,body').animate({scrollTop:$(this.hash).offset().top}, 600);
    });

    //show current page
    var title =	$(document).attr('title');
    olark('api.chat.onBeginConversation', function(){
        olark('api.chat.sendNotificationToOperator', {body: "viewing " + title})
    });
    olark.identify('7960-903-10-2178');/*]]>{/literal}*/

//remove space in email input
    $(function(){
        var txt = $("input#UserName");
        var func = function(e) {
            if(e.keyCode === 32){
                txt.val(txt.val().replace(/\s/g, ''));
            }
        };
        txt.keyup(func).blur(func);
    });

//fancybox
    jQuery(".fancybox-media").fancybox({
        helpers : {
            media: true
        },
        youtube : {
            autoplay: 1
        },
        padding : 0,
        closeBtn: false,
        close  : [27], // escape key
        toggle : [70],  // letter "f" - toggle fullscreen
        height: 480,
        width: 853,
        showNavArrows : false
        //interval: 0,


    });


    $(window).scroll(function () {
        if ($(this).scrollTop() < 1344) {
            $('.fixed-sider').stop().animate({opacity: 0 , visibility : "hidden"}, 600);
        }
        if ($(this).scrollTop() > 1344) {
            $('.fixed-sider').stop().animate({opacity: 1, visibility : "visible"}, 600);
        }
    });

    //----navbar
    try{
        // Do our DOM lookups beforehand
        var nav_container = $("#demo_top_wrapper");
        var nav = $("#sticky_navigation");
        nav_container.waypoint({
            handler: function(event, direction) {
                nav.toggleClass('sticky', direction=='down');
                if (direction == 'down') nav_container.css({ 'height':nav.outerHeight() });
                else nav_container.css({ 'height':'auto' });
            },
            offset: 0
        });
        $.waypoints.settings.scrollThrottle = 0;
        $('#newtoppanel').waypoint(function(event, direction) {
            $('.top').toggleClass('hidden', direction === "up");
        }, {
            offset: 0
        }).find('#demo_top_wrapper').waypoint(function(event, direction) {
                $(this).parent().toggleClass('sticky', direction === "down");
                event.stopPropagation();
            });
    }catch(e){

    }

});

