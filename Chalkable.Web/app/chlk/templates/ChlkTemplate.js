REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.templates', function () {
    "use strict";

    var Autolinker = window.Autolinker || null;

    if (!Autolinker)
        throw Error('Autolinker.js is required');

    /** @class chlk.templates.ChlkTemplate */
    CLASS(
        'ChlkTemplate', EXTENDS(chlk.templates.JadeTemplate), [
            [[Object, Number]],
            String, function getStudentPictureURL(id, sizeW_, sizeH_, notDepartmentSpecific_){
                if(!id)
                    return null;
                var url = this.isDemoSchool() ? window.demoAzurePictureUrl : window.azurePictureUrl;

                var districtId = this.isDemoSchool() ? window.DEMO_SCHOOL_PICTURE_DISTRICT : window.districtId;

                if (notDepartmentSpecific_ == null)
                    url += districtId + '_';
                url += id.valueOf();

                return this.formatPictureURL_(url, sizeW_, sizeH_);
            },

            function getClosestSize_(size, picturesSizes_){
                var pictureSizes = picturesSizes_ || [40, 47, 55, 64, 70, 128, 256];
                if(pictureSizes.indexOf(size) > -1)
                    return size;
                var res = pictureSizes[pictureSizes.length - 1];
                for(var i = 0; i < pictureSizes.length; i++){
                    if(pictureSizes[i] > size){
                        res = pictureSizes[i];
                        break;
                    }
                }
                return res;
            },

            [[String, Number, Number]],
            String, function formatPictureURL_(url, sizeW_, sizeH_, noClosest_)
            {
                if(!noClosest_){
                    sizeW_ = sizeW_ &&  this.getClosestSize_(sizeW_);
                    sizeH_ = sizeH_ &&  this.getClosestSize_(sizeH_);
                }

                if (sizeW_ && sizeH_)
                    return url + '-' + sizeW_ + 'x' + sizeH_;
                if (sizeW_)
                    return url + '-' + sizeW_ + 'x' + sizeW_;
                return url;
            },

            [[String, Number, Number]],
            String, function formatClassPictureURL_(url, sizeW_, sizeH_)
            {
                sizeW_ = sizeW_ &&  this.getClosestSize_(sizeW_, [45, 92]);
                sizeH_ = sizeH_ &&  this.getClosestSize_(sizeH_, [80, 164]);

                if (sizeW_ && sizeH_)
                    return url + '-' + sizeW_ + 'x' + sizeH_;
                if (sizeW_)
                    return url + '-' + sizeW_ + 'x' + sizeW_;
                return url;
            },

            [[Object, Number]],
            String, function getPictureURL(id, sizeW_, sizeH_, notDepartmentSpecific_){
                if(!id)
                    return null;
                var url = window.azurePictureUrl + id.valueOf();
                return this.formatPictureURL_(url, sizeW_, sizeH_, true);
            },

            [[Object, Number]],
            String, function getClassPictureURL(id, sizeW_, sizeH_, notDepartmentSpecific_){
                if(!id)
                    return null;
                var url = window.azurePictureUrl + id.valueOf();
                return this.formatClassPictureURL_(url, sizeW_, sizeH_);
            },

            [[Object, Number]],
            String, function getAppPictureURL(id, sizeW_, sizeH_){
               return this.getPictureURL(id, sizeW_, sizeH_);
            },

            Boolean, function isDemoSchool(){
                return !!window.DEMO_SCHOOL || false;
            },

            [[Number]],
            String, function getSerial(number){
                return window.getSerial(number);
            },

            [[String]],
            String, function getRoleController(role){
                var controller = role.toLowerCase() + 's';
                if (controller.indexOf('admin') > -1)
                    controller = 'admins';
                return controller;
            },

            chlk.models.common.Role, 'userRole',

            chlk.models.people.User, 'currentUser',

            Boolean, 'studyCenterEnabled',

            Boolean, 'LEIntegrated',

            Boolean, 'assessmentEnabled',

            Date, function getServerDate(str_, a_, b_){
                return chlk.models.common.ChlkDate.GET_SERVER_DATE(str_, a_, b_);
            },

            Date, function getSchoolYearServerDate(str_, a_, b_){
                return chlk.models.common.ChlkSchoolYearDate.GET_SCHOOL_YEAR_SERVER_DATE(str_, a_, b_);
            },

            [[String]],
            String, function linkify(text) {
                return Autolinker.link(String(text || ''), {
                    "newWindow": true,
                    truncate: 80,
                    className: 'extenal'
                });
            }
        ])
});
