

from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        #data that needed for filtering by lesson plans for grading period 1
        self.settings_data_lesson_plan = {'announcementType':'3', 'sortType':'0', 'gradingPeriodId':self.gr_periods()[0]}
        self.post('/Feed/SetSettings.json?', self.settings_data_lesson_plan)   
        
        #get 9 'done' lesson plans of the grading period 1
        done_lesson_plans = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(250))
        for_lesson_plan_id_1 = done_lesson_plans['data']['annoucementviewdatas']

        self.lesson_plan_and_activities_id_for_grading_period_1 = []
        self.lesson_plan_and_activities_id_for_grading_period_2 = []
        
        #making all of these lesson plans 'unmark'
        for item in for_lesson_plan_id_1:
            id = str(item['id'])
            type = str(item['type'])
            self.lesson_plan_and_activities_id_for_grading_period_1.append(id)  #id for lesson plans and activities - grading period 1     
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})
        
        #data that needed for filtering by lesson plans for grading period 2
        self.settings_data_lesson_plan_2 = {'announcementType':'3', 'sortType':'0', 'gradingPeriodId':self.gr_periods()[1]}
        self.post('/Feed/SetSettings.json?', self.settings_data_lesson_plan_2)
        
        #get 9 'done' lesson plans of the grading period 2
        done_lesson_plans_2 = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(250))
        for_lesson_plan_id_2 = done_lesson_plans_2['data']['annoucementviewdatas']       
        
        #making all of these lesson plans 'unmark'
        for item in for_lesson_plan_id_2:
            id = str(item['id'])
            type = str(item['type'])
            self.lesson_plan_and_activities_id_for_grading_period_2.append(id)
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})           

        #data that needed for filtering by activities for grading period 1
        self.settings_data_activity = {'announcementType':'1', 'sortType':'0','gradingPeriodId':self.gr_periods()[0]}
        self.post('/Feed/SetSettings.json?', self.settings_data_activity)   
        
        #get 9 'done' activities of the grading period 1
        done_items = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(250))
        for_item_id_1 = done_items['data']['annoucementviewdatas']        
        
        #making all of these activities 'unmark'
        for item in for_item_id_1:
            id = str(item['id'])
            type = str(item['type'])
            self.lesson_plan_and_activities_id_for_grading_period_1.append(id) #id for lesson plans and activities - grading period 1
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})    
        
        #data that needed for filtering by activities for grading period 2
        self.settings_data_activity_2 = {'announcementType':'1', 'sortType':'0','gradingPeriodId':self.gr_periods()[1]}
        self.post('/Feed/SetSettings.json?', self.settings_data_activity_2)   
        
        #get 9 'done' activities of the grading period 2
        done_items_2 = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(250))
        for_item_id_2 = done_items_2['data']['annoucementviewdatas']        
        
        #making all of these activities 'unmark'
        for item in for_item_id_2:
            id = str(item['id'])
            type = str(item['type'])
            self.lesson_plan_and_activities_id_for_grading_period_2.append(id)
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'false'})        

        #reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)

        ##########################################################################################
        
        current_time = time.strftime('%Y-%m-%d')     
        Date = datetime.strptime(current_time, "%Y-%m-%d")
        StartDate = Date.today() - timedelta(days=5)
        EndDate = Date.today() + timedelta(days=5)
        self.current_date_minus_5 = StartDate.strftime("%Y-%m-%d")
        #print self.current_date_minus_5
        self.current_date_plus_5 = EndDate.strftime("%Y-%m-%d")
        
        self.settings_data = {'sortType':'0', 'fromDate': self.current_date_minus_5, 'toDate':self.current_date_plus_5}
        
        self.post('/Feed/SetSettings.json?', self.settings_data)   
    
        self.do_feed_list_and_verify(0)    

    def do_feed_list_and_verify(self, start, count=500):
        dictionary_get_items = self.get('/Feed/List.json?start='+str(start)+'&classId=&complete=false&count='+str(count))
        dictionary_verify_settingsforfeed = dictionary_get_items['data']['settingsforfeed']
        self.assertTrue('announcementtype' in dictionary_verify_settingsforfeed, 'announcementtype exists')
        self.assertTrue('sorttype' in dictionary_verify_settingsforfeed, 'sorttype exists')
        self.assertTrue('gradingperiodid' in dictionary_verify_settingsforfeed, 'announcementtype exists')
        
        announcementtype = str(dictionary_verify_settingsforfeed['announcementtype'])
        sorttype = str(dictionary_verify_settingsforfeed['sorttype'])
        gradingperiodid = str(dictionary_verify_settingsforfeed['gradingperiodid'])
    
        self.assertFalse(set(self.lesson_plan_and_activities_id_for_grading_period_2) & set(self.lesson_plan_and_activities_id_for_grading_period_1), 'Items exits in different grading perios')
    
        dictionary_verify_annoucementviewdatas_all = dictionary_get_items['data']['annoucementviewdatas']
        if len(dictionary_verify_annoucementviewdatas_all) > 0:
            for item in dictionary_verify_annoucementviewdatas_all:
                type = str(item['type'])
                if type == '3':
                    lessonplandata = item['lessonplandata']
                    for key, value in lessonplandata.iteritems():
                        startdate = lessonplandata['startdate']
                        enddate = lessonplandata['enddate']
                        self.assertTrue(self.current_date_minus_5 <= startdate <= self.current_date_plus_5, 'Start date of a lesson plan is correct')
                if type == '1':
                    classannouncementdata = item ['classannouncementdata']
                    for key2, value2 in classannouncementdata.iteritems():
                        expiresdate = classannouncementdata['expiresdate']
                        self.assertTrue(self.current_date_minus_5 <= expiresdate <= self.current_date_plus_5, 'Current date > expiresdate of an activity')
        else:
            self.assertTrue(len(dictionary_verify_annoucementviewdatas_all) == 0, 'There are no items!')
    
    def tearDown(self):
        #reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)
        
        #marking all items as 'done'
        #get_all_unmarket_items = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(2000)) 
        self.settings_data = {'option':'3'}
        self.post('/Announcement/Done.json?', self.settings_data) 
            
if __name__ == '__main__':
    unittest.main()