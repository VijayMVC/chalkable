function HtmlEncode(s)
{
    var el = document.createElement("div");
    el.innerHTML = s;
    s = el.innerHTML;
    delete el;
    return s;
}

function joinUrls(){
    function joinTwo(a, b){
        if(!a){
            return b;
        }
        if(!b){
            return a;
        }
        if((a[a.length-1] == '/')){
            a = a.slice(0, -1);
        }
        if((b[0] == '/')){
            b = b.slice(1);
        }
       return a + '/' + b;
    }

    var res = '';
    [].slice.call(arguments).forEach(function (arg){
        res = joinTwo(res, arg);
    });
    return res;
}

// CHALKABLE MESSAGES ------------------

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
        ((o.buttons && o.buttons.length) ? '<div id="buttons-container" class="' + ((o.containerCls) ? o.containerCls : '') + '"></div>' : '') + (o.newStyle ? '</div>' : '')+
        (o.tip ? '<div class="tip">' + o.tip + '</div>' : '') +
        '</div>');

    o.afterLoad && o.afterLoad();

    o.buttons && o.buttons.forEach(function(el, index){
        new chlkButton({
            cls: el.cls || 'rounded-state-button blue2 x-btn ' + (el.startCls ? el.startCls : ''),
            id: el.id || '',
            enableToggle: el.enableToggle || false,
            pressed: el.pressed || false,
            text: el.text || ((index == 0) ? 'OK' : 'CANCEL'),
            renderTo :'buttons-container',
            toggleGroup: el.toggleGroup,
            handler: function(){
                if(el.handler){
                    o.beforeClose && close();
                    el.handler();
                    !el.notClose && close();
                }else{
                    !el.notClose && close();
                }
            },
            scope: this
        });
    });

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

function addInfoMessage(o){
    addMessage({
        html: (o.header ? '<h2 ' + (o.leftAlign ? 'style="text-align:left"' : '') + '>' + o.header + '</h2>': '')+
            '<p>' + o.text + '</p>',
        cls: 'new-style',
        newStyle: true,
        buttons: [{
            text: o.buttonText || 'OK',
            cls: 'rounded-state-button ' + (o.blueButton ? 'blue2' : 'green2') + ' x-btn',
            handler: o.handler
        }]
    });
}

// END CHALKABLE MESSAGES ------------------

// CHALKABLE TOOLTIP --------------------

jQuery(document).on('mouseover', '[data-tooltip]', function(){
    var node = jQuery(this), tooltip = jQuery('#chlk-tooltip-item'), offset = node.offset();
    tooltip.show();
    tooltip.find('.tooltip-content').html(node.data('tooltip'));
    tooltip.css('left', offset.left + (node.width() - tooltip.width())/2)
        .css('top', offset.top - tooltip.height())
});

jQuery(document).on('mouseleave', '[data-tooltip]', function(){
    var tooltip = jQuery('#chlk-tooltip-item');
    tooltip.hide();
    tooltip.find('.tooltip-content').html('');
});

// END CHALKABLE TOOLTIP --------------------