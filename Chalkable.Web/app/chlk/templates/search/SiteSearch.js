REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.search.SearchItem');

NAMESPACE('chlk.templates.search', function () {

    /** @class chlk.templates.search.SiteSearch*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/sidebars/SiteSearch.jade')],
        [ria.templates.ModelBind(chlk.models.search.SearchItem)],
        'SiteSearch', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'announcementId',

            [ria.templates.ModelPropertyBind],
            String, 'description',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.ShortUserInfo, 'personInfo',

            [ria.templates.ModelPropertyBind],
            chlk.models.search.SearchTypeEnum, 'searchType',

            [ria.templates.ModelPropertyBind],
            Number, 'announcementType',

            [ria.templates.ModelPropertyBind],
            Boolean, 'adminAnnouncement',

            [ria.templates.ModelPropertyBind],
            String, 'smallPictureId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.DepartmentId, 'departmentId',

            [ria.templates.ModelPropertyBind],
            String, 'documentThumbnailUrl',

            String, 'query',

            function getAttachmentUrl(){
                return "/AnnouncementAttachment/DownloadAttachment.json?width=47&height=47&needsDownload=false&announcementAttachmentId=" + this.id
            },

            function getHighlightedText(text){
                var query = this.getQuery().toLowerCase(), textLower = text.toLowerCase(), res = '';

                var startIndexes = [], endIndexes = [], i = -1, len = text.length, qLen = query.length, lastIndex;

                while ((i = textLower.indexOf(query, i+1)) != -1){
                    lastIndex = startIndexes.length - 1;
                    if(lastIndex >= 0 && i <= endIndexes[lastIndex])
                        endIndexes[lastIndex] = i+qLen;
                    else{
                        startIndexes.push(i);endIndexes.push(i+qLen);
                    }

                }

                for(i = 0; i < len; i++){
                    if(startIndexes.indexOf(i) > -1)
                        res += '<strong>';

                    res += text[i];

                    if(endIndexes.indexOf(i + 1) > -1)
                        res += '</strong>';
                }

                return res
            }

        ])
});

