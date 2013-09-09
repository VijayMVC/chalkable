REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.CarouselControl */
    CLASS(
        'CarouselControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/carousel.jade')(this);
            },


            /*
             .PRIVATE(function updateCarousel(data, index, overwrite, nap){
             var panel = jQuery('#app-top-panel'), a;

             if(overwrite){
             caruselTpl.overwrite(this.carusel.body, data);
             a = panel.find('.img').find('a');
             this.currentImg = a;

             }else{
             a = panel.find('.img').find('a');
             this.currentImg = a;
             a.css('width', '1310px');
             var old = panel.find('#app-img-market');
             this.oldImg = old;
             old.css('vertical-align', 'top');
             var img = jQuery(images[index]);
             if(nap < 0){
             a.prepend(img);
             a.css('margin-left', '-655px');
             }else{
             a.append(img);
             }
             var height = 655 / img.width() * img.height();
             img.css('margin-top', -(height - 215)/2 );
             a.attr('title', data.name);
             a.attr('href', '#applications/install/' + data.id);
             a.animate({
             marginLeft: nap > 0 ? -655 : 0
             }, 200, this.afterAnimate.bind(this));
             }
             this.currentImg.removeClass('container-ajax-loader');
             panel.find('.dot.active').removeClass('active');
             panel.find('.dot[number="' + index + '"]').addClass('active');
             switch(this.datalength){
             case 0 : panel.find('.dot').hide();panel.find('.picture-button').hide();break;
             case 1 : panel.find('.dot.one').hide();panel.find('.picture-button').hide();break;
             case 2 : panel.find('.dot.two').hide();break;
             }
             })


             this.appStore.on('load', function(store){
             if(this.firstLoad){
             var imagesData = [];
             store.data.items.forEach(function(el){
             if(el.data.picturesid && el.data.picturesid.length){
             imagesData.push(el.data);
             }
             });
             var max = imagesData.length > 3 ? 2 : imagesData.length - 1;
             this.datalength = imagesData.length;
             if(imagesData[0]){
             this.updateCarousel(imagesData[0], 0, true, false);
             }else{
             jQuery('.picture-container').remove();
             }
             function someFunc(){
             var height = 655 / this.width() * this.height();
             this.remove();
             if(height > 215){
             jQuery('#app-img-market').css('margin-top', -(height - 215)/2 );
             }
             jQuery('#app-img-market').css('visibility', 'visible');
             }

             for (var i = 0 ; i <= max; i++){
             img = new Image();
             img.src = imagesData[i].picturehref;
             img.id = 'app-img-market';
             jQuery(img).attr('number', imagesData.id);
             var clone = jQuery(img).clone();
             if(!i){
             jQuery('body').append(clone);
             clone.css('visibility', 'hidden');
             if(clone.width()){
             someFunc.call(clone);
             }else{
             clone.load(function(img, i){
             someFunc.call(this);
             }.bind(clone, img, i));
             }
             }
             images.push(img);
             }

             function beforeAnimation(){
             if(this.currentImg){
             this.currentImg.stop();
             this.afterAnimate();
             }
             }
             var panel = jQuery('#app-top-panel');
             panel.off('click', '.picture-button');
             panel.on('click', '.picture-button',function(){
             if(max){
             beforeAnimation.apply(that);
             var left = -1;
             if(jQuery(this).hasClass('picture-left')){
             currentIndex = (currentIndex) ? currentIndex - 1 : max;
             }else{
             currentIndex = (currentIndex == max) ? 0 : currentIndex + 1;
             left = +1;
             }
             that.updateCarousel(imagesData[currentIndex], currentIndex, false, left);
             }
             return false;
             });
             panel.off('click', '.dot:not(.active)');
             panel.on('click', '.dot:not(.active)',function(){
             if(max){
             beforeAnimation.apply(that);
             var lastIndex = currentIndex;
             var node = jQuery(this);
             panel.find('.dot.active').removeClass('active');
             node.addClass('active');
             currentIndex = parseInt(node.attr('number'));
             that.updateCarousel(imagesData[currentIndex], currentIndex, false, currentIndex - lastIndex);
             }
             return false;
             });
             this.firstLoad = false;
             }
             }.bind(this));
             })
            * */
        ]);
});