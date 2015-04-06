REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.standard', function () {
    "use strict";
    var SJX = ria.serialize.SJX;

    /** @class chlk.models.standard.StandardTreeItem*/
    CLASS(
        'StandardTreeItem', EXTENDS(chlk.models.standard.Standard), [

            chlk.models.id.ClassId, 'classId',
            ArrayOf(chlk.models.standard.Standard), 'standardChildren',

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.standardChildren = SJX.fromArrayOfDeserializables(raw.standardchildren, chlk.models.standard.StandardTreeItem);
            },

            Boolean, function hasChildren(){
                return this.getStandardChildren() && this.getStandardChildren().length > 0;
            },

            ArrayOf(chlk.models.standard.Standard), function getLastLeafs(){
                if(this.hasChildren()){
                    var children = this.getStandardChildren().filter(function(item){return item.hasChildren && item.hasChildren();});
                    return children.length > 0 && children[0].getLastLeafs  ? children[0].getLastLeafs() : this.getStandardChildren();
                }
                return [];
            },


            [[String, chlk.models.id.ClassId, chlk.models.id.StandardSubjectId, ArrayOf(chlk.models.standard.Standard), chlk.models.id.AnnouncementId]],
            function $(description_, classId_, subjectId_, standardChildren_, announcementId_){
                BASE();
                if(standardChildren_)
                    this.setStandardChildren(standardChildren_);
                if(classId_)
                    this.setClassId(classId_);
                if(subjectId_)
                    this.setSubjectId(subjectId_);
                if(description_)
                    this.setDescription(description_);
                if(announcementId_)
                    this.setAnnouncementId(announcementId_);
            }
    ]);
});
