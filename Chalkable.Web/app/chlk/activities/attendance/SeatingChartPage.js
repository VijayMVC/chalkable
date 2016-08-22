REQUIRE('chlk.activities.attendance.BasePostAttendancePage');
REQUIRE('chlk.templates.attendance.SeatingChartTpl');
REQUIRE('chlk.templates.attendance.SeatingChartPeopleTpl');
REQUIRE('chlk.templates.attendance.ClassAttendanceWithSeatPlaceTpl');
REQUIRE('chlk.templates.attendance.NotTakenAttendanceClassesTpl');

NAMESPACE('chlk.activities.attendance', function () {
    "use strict";
    
    var needChartPopUp = false;

    var draggableOptions = {
        revert: true,
        revertDuration: 0,
        cancel: '.empty-box'
    };

    var activeDragging = false;

    var droppableOptions = {
        activeClass: "active",
        hoverClass: "hover",

        drop: function(event, ui) {
            var $uiDroppable = jQuery('.ui-droppable'),
                $studentBox = $uiDroppable.find('.student-block').first();

            var $node = $(this);
            var draggable = ui.draggable,
                topPos = event.pageY,
                leftPos = event.pageX,
                toolbarLeftOffset = (parseInt($node.parents('.second-container').css('left'), 10) || 0),
                uidrOffset = $uiDroppable.offset(),
                uidrLeft = uidrOffset.left,
                top = uidrOffset.top,
                horizontal = $studentBox.outerWidth(true),
                vertical = $studentBox.outerHeight(true),
                width = $studentBox.innerWidth(),
                height = $studentBox.innerHeight(),
                topPadding = vertical - height,
                leftPadding = horizontal - width;

            var vKil = Math.floor((leftPos - uidrLeft)/horizontal);
            var hKil = Math.floor((topPos - top)/vertical);

            var columns = $node.data('columns');
            var index = columns * hKil + vKil;
            var newTop = top + hKil * vertical, diffTop = topPos - newTop;
            var newLeft = uidrLeft + vKil * horizontal, diffLeft = leftPos - newLeft;

            if((diffTop > topPadding) && (diffTop < topPadding + height) && (diffLeft > leftPadding) && (diffLeft < leftPadding + width)){
                jQuery('#submit-chart').show(100);
                needChartPopUp = true;
                jQuery('.seating-chart-content').addClass('edited');
                var droppable = $(this).find('.student-block[data-index=' + index + ']');
                if(droppable.find('.empty')[0]){
                    droppable.html(draggable.html()).removeClass('empty-box');
                    if(!droppable.next('.absolute')[0])
                        jQuery('<div class="empty-box student-block absolute"><div class="empty"></div></div>').insertAfter(droppable);
                    if(draggable.parents('.seating-chart-people')[0]){
                        draggable.css('opacity', 0);
                        draggable.animate({
                            width: 0
                        }, 250, function(){
                            draggable.remove();
                            checkPadding();
                        });
                    }else{
                        setEmptyBoxHtml(new ria.dom.Dom(draggable));
                        try{
                            //draggable.draggable('destroy');
                        }catch(e){

                        }
                    }
                    try{
                        droppable.draggable(draggableOptions);
                    }catch(e){

                    }
                }else{
                    var html = droppable.html();
                    droppable.html(draggable.html());
                    draggable.html(html);
                }
                var buttons = new ria.dom.Dom('.save-attendances-buttons').find('a');
                if(new ria.dom.Dom('.seating-chart-container').find('.not-empty').exists())
                    buttons.removeClass('disabled');
            }



        }
    };

    function setEmptyBoxHtml(node){
        var model = new chlk.models.attendance.ClassAttendance();
        var tpl = new chlk.templates.attendance.ClassAttendanceWithSeatPlaceTpl();
        tpl.assign(model);
        node.setHTML(tpl.render()).addClass('empty-box');
        var next = node.next('.absolute');
        next.exists() && next.remove();
    }

    function checkPadding(){
        var body = jQuery('body');
        var padding = parseInt(body.css('padding-bottom'), 10);
        var height = body.find('.seating-chart-people').height();
        if(!padding || padding != height){
            body.css('padding-bottom', height);
        }
    }

    /** @class chlk.activities.attendance.SeatingChartPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.attendance.SeatingChartTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.SeatingChartTpl, '', null, ria.mvc.PartialUpdateRuleActions.Replace)],
        'SeatingChartPage', EXTENDS(chlk.activities.attendance.BasePostAttendancePage), [
            function $(){
                BASE();
                needChartPopUp = false;
                this._submitFormSelector = '.save-attendances-form';
                this._canChangeReasons = null;
            },

            [[ria.dom.Dom]],
            OVERRIDE, function tryToLeave(node){
                if(this._needPopUp && !node.parent(this._submitFormSelector).exists() || needChartPopUp && !node.is('#submit-chart')){
                    this.showLeavePopUp(node, needChartPopUp);
                    return false;
                }
                return true;
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.attendance.SeatingChartTpl, 'savedChart')],
            VOID, function updateSeatingChart(tpl, model, msg_) {
                var button = this.dom.find('.add-remove-students');
                var addBlock = this.dom.find('.add');
                if(this.dom.find('.student-block:not(".empty-box")').count()){
                    button.setHTML(Msg.Edit_students);
                    addBlock.removeClass('black-arrow').setHTML('');
                }else{
                    button.setHTML(Msg.Add_students);
                    addBlock.addClass('black-arrow').setHTML(Msg.Add_your_students);
                }
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.attendance.NotTakenAttendanceClassesTpl)],
            VOID, function doUpdateClasses(tpl, model, msg_) {
                var dom = ria.dom.Dom();
                model.getItems().forEach(function(item){
                    dom.find('.alerts-icon[data-id=' + item.getId() + ']').show();
                })
            },

            function checkEqualsAttendances(){
                var needPopUp = false;
                this.dom.find('.attendance-data').forEach(function(item){
                    if(!needPopUp){
                        var oldLevel = item.getData('old-level'),
                            level = item.getData('level'),
                            oldReasonId = item.getData('old-reason-id'),
                            reasonId = item.getData('reason-id');
                        if(((level || oldLevel) && (level != oldLevel)) || ((reasonId || oldReasonId) && (reasonId != oldReasonId)))
                            needPopUp = true;
                    }
                });
                this._needPopUp = needPopUp;
            },

            chlk.models.attendance.SeatingChart, 'model',

            Array, function getAttendances_(){
                var res = [];
                var attendancesNodes = new ria.dom.Dom('.attendance-data');
                attendancesNodes.forEach(function(node){
                    res.push({
                        personId: node.getData('id'),
                        type: node.getData('type'),
                        level: node.getData('level'),
                        attendanceReasonId: node.getData('reason-id'),
                        dailyAttendancePeriod: node.getData('is-daily-attendance-period')
                    });
                });
                return res;
            },

            [ria.mvc.DomEventBind('click', '.add-remove-students')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addRemoveStudentsClick(node, event){
                if(!new ria.dom.Dom('.seating-chart-people:visible').exists()){
                    var chart = new ria.dom.Dom('.seating-chart-people');
                    chart.show();
                    this.dom.addClass('dragging-on');
                    if(this.dom.find('.seating-chart-content').hasClass('edited')){
                        this.dom.find('#submit-chart').show(100);
                        needChartPopUp = true;
                    }

                    $(".draggable:not(.empty-box)").draggable(draggableOptions);
                    $(".droppable").droppable(droppableOptions);
                    activeDragging = true;
                    setTimeout(function(){
                        var body = jQuery('body');
                        var padding = parseInt(body.css('padding-bottom'), 10);
                        var height = body.find('.seating-chart-people').height();
                        if(!padding || padding < height){
                            if(padding)
                                body.data('padding', padding);
                            body.css('padding-bottom', height);
                        }
                    }, 1);
                }else{
                    this.stopDragging();
                }
            },

            function closePopUp(){
                this.dom.find('.seating-chart-popup')
                    .hide()
                    .removeClass('absent')
                    .removeClass('late')
                    .setCss('top', 'auto');
                this.dom.find('.active-student').removeClass('active-student')
            },

            [ria.mvc.DomEventBind('click', '.attendance-data')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function studentClick(node, event){
                var isReadOnly = node.getData('is-read-only');
                if(!this.dom.hasClass('dragging-on') && !isReadOnly && this.dom.find('.page-content').hasClass('can-post')){
                    var popUp = this.dom.find('.seating-chart-popup');
                    var main = node.parent('.seating-chart-content');
                    var bottom = main.height() + main.offset().top - node.offset().top;
                    var left = node.offset().left - main.offset().left + 10;
                    popUp.setCss('bottom', bottom);
                    popUp.setCss('left', left);
                    var reasonId = node.getData('reason-id') || -1;
                    var reasonText = node.getData('reason-text') || '';
                    var type = node.getData('type');
                    popUp.find('.selected').removeClass('selected');
                    popUp.find('.reason-text').setHTML('');
                    popUp.find('.first-part .item[data-type=' + type + ']')
                        .addClass('selected')
                        .find('.reason-text')
                        .setHTML(' - ' + reasonText);
                    popUp.find('.type-part[data-type=' + type + ']')
                        .find('.item[data-id=' + reasonId + ']')
                        .addClass('selected');
                    setTimeout(function(){
                        node.addClass('active-student');
                        popUp.show();
                        this.checkPopUp();
                    }.bind(this), 1);
                }
            },

            function checkPopUp(){
                var popUp = this.dom.find('.seating-chart-popup');
                if(popUp.offset().top < 0){
                    popUp.setCss('bottom', 'auto');
                    popUp.setCss('top', 0);
                }
            },

            [ria.mvc.DomEventBind('click', '.first-part .item')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function popUpClick(node, event){
                var typeId = node.getData('type');
                var popUp = node.parent('.seating-chart-popup');
                switch(typeId){
                    case chlk.models.attendance.AttendanceTypeEnum.ABSENT.valueOf():
                        if(this._canChangeReasons){
                            popUp.addClass('absent'); this.checkPopUp();
                        }
                        else
                            this.setTypeByNode(node.parent('.seating-chart-popup').find('.absent.reason-item:eq(0)'));
                        break;
                    case chlk.models.attendance.AttendanceTypeEnum.LATE.valueOf():
                        if(this._canChangeReasons){
                            popUp.addClass('late'); this.checkPopUp();
                        }
                        else
                            this.setTypeByNode(node.parent('.seating-chart-popup').find('.late.reason-item:eq(0)'));
                        break;
                    default:
                        this.setTypeByNode(node);
                }
            },

            function setTypeByNode(node){
                var reasonId = node.getData('id');
                var typeId = node.getData('type');
                var level = node.getData('level');
                var reasonText = node.find('.text').getHTML();
                var studentData = this.dom.find('.active-student');
                studentData.setData('type', typeId);
                studentData.setData('level', level);
                studentData.setData('reason-id', reasonId);
                studentData.setData('reason-text', reasonText);
                var parent = studentData.parent('.not-empty');
                parent.removeClass('absent')
                    .removeClass('present')
                    .removeClass('late');
                switch(typeId){
                    case chlk.models.attendance.AttendanceTypeEnum.ABSENT.valueOf():
                        parent.addClass('absent'); break;
                    case chlk.models.attendance.AttendanceTypeEnum.LATE.valueOf():
                        parent.addClass('late'); break;
                    default:
                        parent.addClass('present');
                }
                parent.find('.reason-text')
                    .setHTML(reasonText)
                    .setData('tooltip', reasonText);
                this.closePopUp();
                this.checkEqualsAttendances();
            },

            [ria.mvc.DomEventBind('click', '.reason-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function reasonClick(node, event){
                this.setTypeByNode(node);
            },

            [ria.mvc.DomEventBind('click', '.remove-student')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function removeStudentClick(node, event){
                this.dom.find('#submit-chart').show(100);
                needChartPopUp = true;
                this.dom.find('.seating-chart-content').addClass('edited');
                var parent = node.parent('.student-block ');
                var clone = parent.clone();
                parent.addClass('empty-box');
                setEmptyBoxHtml(parent);
                var container = new ria.dom.Dom('.seating-chart-people')
                    .find('.people-container');
                clone.prependTo(container)
                    .removeClass('droppable')
                    .removeClass('ui-droppable');
                jQuery(clone.valueOf()).draggable(draggableOptions);
                checkPadding();
                var buttons = this.dom.find('.save-attendances-buttons').find('a');
                if(!this.dom.find('.seating-chart-container').find('.not-empty').exists())
                    buttons.addClass('disabled');
            },

            [ria.mvc.DomEventBind('click', '#all-present-link:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function allPresentClick(node, event){
                this._needPopUp = true;
                new ria.dom.Dom('.student-block').forEach(function(node){
                    var container = node.find('.not-empty');

                    if(container.find('.attendance-data').getData("is-read-only")) return;

                    if(container.exists()){
                        container
                            .removeClass('absent')
                            .removeClass('late')
                            .addClass('present');
                        var dataNode = container.find('.attendance-data');
                        dataNode.setData('type', chlk.models.attendance.AttendanceTypeEnum.PRESENT.valueOf());
                        dataNode.setData('level', null);
                        dataNode.setData('old-level', null);
                        dataNode.setData('reason-id', null);
                        dataNode.setData('old-reason-id', null);
                    }
                });
            },

            function recalculateChartInfo(){
                var rows = jQuery('.seating-row').length;
                var columns = jQuery('.seating-row:eq(0)').find('.student-block[data-index]').length;
                var classId = this.dom.find('.class-id').getValue();
                var seatingList = [], minRows = 0, minColumns = 0;
                jQuery('.seating-row').each(function(index, items){
                    var seatings = [];
                    jQuery(items).find('.student-block[data-index]').each(function(){
                        var node = jQuery(this);
                        var node2 = node.find('.image-container'),
                            row = node.data('row'),
                            column = node.data('column');
                        seatings.push({
                            index: node.data('index'),
                            row: row,
                            column: column,
                            studentId: node2[0] ? node2.data('id') : null
                        });
                        if(node2[0]){
                            if(row > minRows)
                                minRows = row;
                            if(column > minColumns)
                                minColumns = column;
                        }
                    });
                    seatingList.push(seatings);
                }.bind(this));

                this.dom.find('#min-rows').setValue(minRows);
                this.dom.find('#min-columns').setValue(minColumns);

                var res = {
                    rows: rows,
                    columns: columns,
                    classId: classId,
                    seatingList: seatingList
                };
                this.dom.find('.text-for-post').setValue(JSON.stringify(res));
            },

            [ria.mvc.DomEventBind('click', '#submit-attendance-button:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function postButtonClick(node, event){
                this.dom.find('.attendances-json').setValue(JSON.stringify(this.getAttendances_()));
                this.dom.find('.save-attendances-form').trigger('submit');
                node.setHTML('SAVING....');
                this.dom.find('.attendance-data').forEach(function(item){
                    item.setData('old-level', item.getData('level'));
                    item.setData('old-reason-id', item.getData('reason-id'));
                });
                this._needPopUp = false;
                node.parent('form').trigger('submit');
                ria.dom.Dom('.class-button.pressed').find('.alerts-icon').fadeOut();
            },

            Boolean, 'ableRePost',

            [ria.mvc.PartialUpdateRule(chlk.templates.attendance.SeatingChartTpl, 'saved')],
            VOID, function updateGradingPart(tpl, model, msg_) {
                if(this.isAbleRePost()){
                    this.dom.find('#submit-attendance-button').setHTML('POST IT');
                }else{
                    this.dom.find('#submit-attendance-button, #all-present-link').remove();
                }
            },

            [ria.mvc.DomEventBind('click', '.update-grid')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function updateGridClick(node, event){
                this.recalculateChartInfo();
            },

            OVERRIDE, VOID, function onPartialRender_(model, msg_){
                BASE(model, msg_);
                this.stopDragging();
            },

            function stopDragging(remove_){
                var chart = new ria.dom.Dom('.seating-chart-people');
                if(remove_)
                    chart.remove();
                else
                    chart.hide();
                this.dom.removeClass('dragging-on');
                this.dom.find('#submit-chart').hide(100);
                if(activeDragging){
                    if($( ".droppable" )[0])
                        try{
                            $( ".droppable" ).droppable( "destroy" );
                        }catch(e){
                            _DEBUG && console.error('droppable', e);
                        }
                    if($( ".draggable:not(.empty-box)" )[0])
                        try{
                            $( ".draggable:not(.empty-box)" ).draggable( "destroy" );
                        }catch(e){
                            _DEBUG && console.error('draggable', e);
                        }
                    activeDragging = false;
                }
                setTimeout(function(){
                    var body = jQuery('body');
                    var padding = body.data('padding');
                    body.css('padding-bottom', padding || 'inherit');
                }, 1);
            },

            function getAttendancesModel(model){
                return model;
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                var attModel = this.getAttendancesModel(model);
                this._canChangeReasons = attModel.isAbleChangeReasons();
                this.setAbleRePost(attModel.isAbleRePost());

                var tpl = new chlk.templates.attendance.SeatingChartPeopleTpl();
                tpl.assign(attModel);
                tpl.renderTo(new ria.dom.Dom('body'));

                new ria.dom.Dom().on('click.seating', '.close-people, #submit-chart', function(node, event){
                    this.stopDragging();
                }.bind(this));

                new ria.dom.Dom().on('click.seating', function(doc, event){
                    var node = new ria.dom.Dom(event.target);
                    if(!node.isOrInside('.seating-chart-popup') && this.dom.find('.seating-chart-popup:visible').exists())
                        this.closePopUp();
                }.bind(this));

                new ria.dom.Dom().on('click.seating', '#submit-chart', function(node, event){
                    needChartPopUp = false;
                    this.recalculateChartInfo();
                    this.dom.find('.seating-chart-content').removeClass('edited');
                    node.hide(100);
                    var postButtons = this.dom.find('#submit-attendance-button, #all-present-link');
                    this.dom.find('.save-chart-form').trigger('submit');
                    if(postButtons.exists() && this.dom.find('.attendance-data').count())
                        postButtons.removeClass('disabled');
                    else
                        postButtons.addClass('disabled');
                }.bind(this));

                jQuery(window).on('resize.seating', function(){
                    if(new ria.dom.Dom('.seating-chart-people').exists())
                        checkPadding();
                });

            },

            OVERRIDE, function leaveClick_(node){
                BASE(node);
                needChartPopUp = false;
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                this.stopDragging(true);
                new ria.dom.Dom().off('click.seating');
                jQuery(window).off('resize.seating');
            }
        ]);
});