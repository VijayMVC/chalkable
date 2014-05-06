navigator.sayswho= (function(){
    var N= navigator.appName, ua= navigator.userAgent, tem;
    var M= ua.match(/(opera|chrome|safari|firefox|msie)\/?\s*(\.?\d+(\.\d+)*)/i);
    if(M && (tem= ua.match(/version\/([\.\d]+)/i))!= null) M[2]= tem[1];
    M= M? [M[1], M[2]]: [N, navigator.appVersion,'-?'];
    return M;
})();

var thisBrowser= (function(){
    var name = navigator.sayswho[0].toLowerCase();
    return {
        isIE: name == 'msie',
        isOpera: name == 'opera',
        isChrome: name == 'chrome',
        isSafari: name == 'safari',
        isFirefox: name == 'firefox'
    }
})();

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

function strcmp(a, b) {
    if (a.toString().toLowerCase() < b.toString().toLowerCase()) return -1;
    if (a.toString().toLowerCase() > b.toString().toLowerCase()) return 1;
    return 0;
}


function HtmlEncode(s)
{
    var el = document.createElement("div");
    el.innerHTML = s;
    s = el.innerHTML;
    delete el;
    return s;
}

function getActionButtons(){
    return [Msg.Password_reset, Msg.New_SIS_info_CSV, Msg.School_setup, Msg.Invite_users];
}

Object.extend = function(destination, source) {
  for (var property in source)
    destination[property] = source[property];
  return destination;
};

function getNoNull(value){
    return (value != null) ? value : '';
}

function daysInMonth(month,year) {
    return new Date(year, month, 0).getDate();
}

function getSerial(number){
    switch(number){
        case 1: return '1st';
        case 2: return '2nd';
        case 3: return '3rd';
        default: return number + 'th';
    }
}

function buildShortText(text, newTextlength){
    newTextlength = newTextlength || 80;
    if(text != null && text.length > newTextlength){
        return text.slice(0, newTextlength) + '...';
    }
    return text;
}

var GradingStyler = {

    gradeLettersReverse: ['A+', 'A', 'A-','B+', 'B', 'B-','C+', 'C', 'C-','D+', 'D', 'D-','F' ],

    getGradeLetters: function(){
        return this.gradeLettersReverse;
    },

    getGradeLettersMap: function(){
        if (!this.gradeLettersMap){
            this.gradeLettersMap = {
                'GradingAbcf': this.gradeLettersReverse.slice().reverse(),
                'GradingCheck': ['C-', 'C', 'C+'],
                'GradingComplete': ['Incomplete', 'Complete']
            };
        }
        return this.gradeLettersMap;
    },
    getGradesOptions: function(mappings, gradingStyle){
        var gradesOptions = '<option>';

        var letters = this.getGradeLettersByGradingStyle(gradingStyle);

        var mapping = mappings['get' + this.getGradingStyle(gradingStyle)];

        for(var i = letters.length; i--;){
            if(mapping[i] != -1){
                gradesOptions = gradesOptions + '<option value="' + i + '">' + letters[i];
            }
        }
        return gradesOptions;
    },

    getGradeValues: function(){
        return [this.gradeLettersReverse, this.getGradeLettersMap()['complete'], this.getGradeLettersMap()['gradingcomplete']];
    },

    getLetterByGrade: function(grade, gradingMappings, gradingStyle){
        if (grade == null || grade == undefined) return ' ';
        if (gradingStyle == 0){
            return getNoNull(grade);
        } else {
            var mapping = gradingMappings['get' + this.getGradingStyle(gradingStyle)]();
            var gradeId = this.getGradeValueId(grade, mapping);
            var letters = this.getGradeLettersByGradingStyle(gradingStyle);

            return letters[gradeId];
        }

    },
    getGradeLettersByGradingStyle: function(gradingStyle){
        return this.getGradeLettersMap()[this.getGradingStyle(gradingStyle)];
    },

    getGradingStyle: function(gradingStyle){
        var GradingStyleIds =
        ['Numeric100', 'Abcf', 'Complete', 'Check'];

        if (gradingStyle > 0 && gradingStyle < 4){
            return 'Grading' + GradingStyleIds[gradingStyle];
        }
        return 'GradingAbcf';
    },

    getGradeValueId: function(value, mapping){
        var res = -1;
        for(var i = 0; i < mapping.length; i++){
            if(value <= mapping[i]){
                res = i;
                break;
            }
        }
        return res;
    },

    getGradeValue: function(value, gradingMappings, gradingStyle){
        var mapping = gradingMappings['get' + this.getGradingStyle(gradingStyle)];
        var res = -1;
        for(var i = 0; i < mapping.length; i++){
            if(value <= mapping[i]){
                res = mapping[i];
                break;
            }
        }
        return res;
    },

    getGradeNumberValue: function (value, gradingMappings, gradingStyle){
        var mapping = gradingMappings['get' + this.getGradingStyle(gradingStyle)]();
        //return mapping[this.getGradeValueId(value, mapping)];
        return mapping[GradingStyler.getGradeLettersMap()[this.getGradingStyle(gradingStyle)].indexOf(value)];
    },

    getFromMapped: function(gradingStyle, value){
        if(gradingStyle){
            return this.getGradeLettersByGradingStyle(gradingStyle)[value];
        }else{
            return value;
        }
    }
};

String.prototype.capitalize = function() {
    return this.charAt(0).toUpperCase() + this.slice(1);
}

function debugPrint(name, obj){
    console.log(name, 'start');
    for (var prop in obj){

        if (obj.hasOwnProperty(prop) && prop.indexOf('get') != -1){
            console.log(prop, obj[prop]());
        }
    }
    console.log(name, 'end');
}