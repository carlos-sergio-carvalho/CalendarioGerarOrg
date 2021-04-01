// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var url = window.location.pathname,
    urlRegExp = new RegExp(url.replace(/\/$/, '') + "$"); // create regexp to match current url pathname and remove trailing slash if present as it could collide with the link in navigation in case trailing slash wasn't present there
// now grab every link from the navigation
$('.menu a').each(function () {
    // and test its normalized href against the url pathname regexp
    if (urlRegExp.test(this.href.replace(/\/$/, ''))) {
        $(this).parent().addClass('active');
        return false; 
    }
});

Date.prototype.toJSONLocal = function () {
    function addZ(n) {
        return (n < 10 ? '0' : '') + n;
    }
    return this.getFullYear() + '-' +
        addZ(this.getMonth() + 1) + '-' +
        addZ(this.getDate());
}; 

var datastest = [
    { datainicial: '2020-08-07', datafinal: '2020-08-08' },
    { datainicial: '2020-08-09', datafinal: '2020-08-10' },
    { datainicial: '2020-08-11', datafinal: '2020-08-12' }
];

// Define a new component called todo-item  { datainicial: $(entry).val(), datafinal: $('.dataF').eq(index).val()}
Vue.component('datainput', {
    props: {
        datas: Array,
        title: String
    },
    template: '<div class="col-sm-4" style="min-width:420px">\
                <div class="form-group ">\
                    <label class="control-label">{{title}}</label>\
                    <div class="input-group">\
                        <input type="date" class="form-control" placeholder="Início" v-model="datai" :max="dataf" @change="dataf=datai"  />\
                        <span class="input-group-addon">-</span>\
                        <input type="date" class="form-control" placeholder="Fim" v-model="dataf" :min="datai" />\
                        <span class="input-group-addon" v-on:click="adddata"><span class="glyphicon glyphicon-plus"></span></span>\
                    </div>\
                </div>\
                <div class="form-group">\
                    <div class="input-group">\
                        <div id="calendarextra0" style="overflow-y:auto;max-height:250px;overflow-x:initial">\
                        \<div class="input-group" v-for="item in listdata"><input type="date" class="form-control" readonly="" placeholder="Início" v-model="item.datainicial"><span class="input-group-addon">-</span><input type="date" class="form-control" readonly="" placeholder="Fim" v-model="item.datafinal" ><span class="input-group-addon" v-on:click="removedata(item)"><span class="glyphicon glyphicon-minus"></span></span></div>\
                        </div>\
                    </div>\
                </div>\
            </div>',

    data() {
        return {
            datai: '',
            dataf: '',
            listdata: this.datas
        }
    },
    methods: {
        adddata: function () {

            if (this.datai != '' && this.dataf != '') {
                //console.log(this.datas);
                this.listdata.push({ datainicial: this.datai, datafinal: this.dataf });
                this.$emit('listchange', this.listdata);
                this.dataf = this.datai = '';
            }
        },
        removedata: function (item) {
            this.listdata = this.listdata.filter(i => {
                return i !== item
            });
            this.$emit('listchange', this.listdata);
        }
    }
})
