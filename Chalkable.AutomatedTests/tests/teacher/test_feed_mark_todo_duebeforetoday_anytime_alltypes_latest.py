# Feed > ToDo. ["Items due before today", "Any time", "All Types", "Latest"]
# http://screencast.com/t/jLpDvTAcpG

from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        self.settings_data_for_mark_undone = {'option':'3'}
        #data that will need for filtering by lesson plans for grading period 1
        self.settings_data_lesson_plan = {'announcementType':'3', 'sortType':'0', 'gradingPeriodId':self.gr_periods()[0]}
        self.post('/Feed/SetSettings.json?', self.settings_data_lesson_plan)   
        
        #get 'done' lesson plans of the grading period 1
        done_lesson_plans_json_unicode = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(250))


        self.id_of_lessonplan_and_activities_gradingperiod1 = []
        self.id_of_lessonplan_and_activities_gradingperiod2 = []
        
        #making all of these lesson plans 'unmark'
        self.post('/Announcement/UnDone.json?', self.settings_data_for_mark_undone)
      
        #data that needed for filtering by lesson plans for grading period 2
        self.settings_data_lesson_plan_2 = {'announcementType':'3', 'sortType':'0', 'gradingPeriodId':self.gr_periods()[1]}
        self.post('/Feed/SetSettings.json?', self.settings_data_lesson_plan_2)
        
        #get 'done' lesson plans of the grading period 2
        done_lesson_plans_json_unicode_2 = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(250))

        
        #making all of these lesson plans 'unmark'
        self.post('/Announcement/UnDone.json?', self.settings_data_for_mark_undone)
            
#########################################################################################################################
        #data that needed for filtering by activities for grading period 1
        self.settings_data_activity = {'announcementType':'1', 'sortType':'0','gradingPeriodId':self.gr_periods()[0]}
        self.post('/Feed/SetSettings.json?', self.settings_data_activity)   
        
        #get 'done' activities of the grading period 1
        done_activities_json_unicode = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(250))

        
        #making all of these activities 'unmark'
        self.post('/Announcement/UnDone.json?', self.settings_data_for_mark_undone)
        
        #data that needed for filtering by activities for grading period 2
        self.settings_data_activity_2 = {'announcementType':'1', 'sortType':'0','gradingPeriodId':self.gr_periods()[1]}
        self.post('/Feed/SetSettings.json?', self.settings_data_activity_2)   
        
        #get 'done' activities of the grading period 2
        done_activities_json_unicode_2 = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=true&count='+str(250))

        
        #making all of these activities 'unmark'
        self.post('/Announcement/UnDone.json?', self.settings_data_for_mark_undone) 
           
#########################################################################################################################

        #reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)
        
      
        #set filter to latest ordering
        self.settings_data = {'sortType':'1'}
        self.post('/Feed/SetSettings.json?', self.settings_data)   
       
        #get items 'Due before today'
        self.settings_data = {'option':'1'}
        self.post('/Announcement/Done.json?', self.settings_data)   
 
        self.do_feed_list_and_verify(0)    

    def do_feed_list_and_verify(self, start, count=1000):
        #get a current time
        self.current_time = time.strftime('%Y-%m-%d')     
    
        list_items_json_unicode = self.get('/Feed/List.json?start='+str(start)+'&classId=&complete=false&count='+str(count))
        dictionary_verify_settingsforfeed = list_items_json_unicode['data']['settingsforfeed']
        self.assertTrue('announcementtype' in dictionary_verify_settingsforfeed, 'announcementtype exists')
        self.assertTrue('sorttype' in dictionary_verify_settingsforfeed, 'sorttype exists')
        self.assertTrue('gradingperiodid' in dictionary_verify_settingsforfeed, 'announcementtype exists')

        announcementtype = str(dictionary_verify_settingsforfeed['announcementtype'])
        sorttype = str(dictionary_verify_settingsforfeed['sorttype'])
        gradingperiodid = str(dictionary_verify_settingsforfeed['gradingperiodid'])
            
        list_for_date = []
        
        dictionary_verify_annoucementviewdatas_all = list_items_json_unicode['data']['annoucementviewdatas']
        if len(dictionary_verify_annoucementviewdatas_all) > 0:
            for item in dictionary_verify_annoucementviewdatas_all:
                type = str(item['type'])
                if type == '3':
                    complete = item['complete']
                    self.assertTrue(complete == False, "Verify item's state on false")
                    lessonplandata = item['lessonplandata']
                    startdate = lessonplandata['startdate']
                    enddate = lessonplandata['enddate']
                    list_for_date.append(startdate)
                    self.assertLessEqual(self.current_time, enddate, 'Current date <= enddate of a lesson plan ' + str(item["id"]) + ": " + self.current_time + " <= " + enddate)

                if type == '1':
                    complete = item['complete']
                    self.assertTrue(complete == False, "Verify item's state on false")
                    classannouncementdata = item ['classannouncementdata']
                    expiresdate = classannouncementdata['expiresdate']
                    list_for_date.append(expiresdate)
                    self.assertLessEqual(self.current_time, expiresdate, 'Current date <= expiresdate of an activity ' + str(item["id"]) + ": " + self.current_time + " <= " + expiresdate)


            decoded_list = [x.encode('utf-8') for x in list_for_date]
            decoded_list_to_str = ', '.join(decoded_list)
            
            sorted_list_dates = sorted(decoded_list, reverse=True)
            reverse_list_to_str = ', '.join(sorted_list_dates)
            
            self.assertTrue(decoded_list == sorted_list_dates, 'Items are sorted not in latest order' + ": " + decoded_list_to_str + ' == ' + reverse_list_to_str)

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