(function () {
    'use strict';

    angular
        .module('spApp.batchJobs.test', [])
        .controller('batchJobCtrl', ['Page', 'commonService', 'dataService', batchJobCtrl]);

    function batchJobCtrl(Page, commonService, dataService){
        var self = this;

        Page.setTitle('Batch Job Test');

        self.username = '';
        self.key = '';
        self.message = '';

        self.alert = {
            msg:'',
            type: '',
            alertObj: ''
        };

        self.run = function() {
            var url = '/api/batchjob/execute?username='+self.username+'&key='+self.key;
            dataService.post(url).then(function (data) {
                if (data.success) {
                    commonService.setAlert(self.alert, 'Success!', 'success');
                } else {
                    commonService.setAlert(self.alert, data.errorMessage, 'danger');
                }
            }, function(response){
                if (response.status > 0) {
                    commonService.setAlert(self.alert, response.data.ExceptionMessage, 'danger');
                }
            });
        };
    }
})();
