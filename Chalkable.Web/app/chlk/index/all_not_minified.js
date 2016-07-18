//All homepage misc. scripts
$(window).load(function () {
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

    $(function() {
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

    $(window).scroll(function () {
        if ($(this).scrollTop() < 1344) {
            $('.fixed-sider').stop().animate({opacity: 0 , visibility : "hidden"}, 600);
        }
        if ($(this).scrollTop() > 1344) {
            $('.fixed-sider').stop().animate({opacity: 1, visibility : "visible"}, 600);
        }
    });
});

