#Feed > All. Verify that all items in a state 'done'. 

from base_auth_test import *


class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        self.do_feed_list_and_verify(0)    

    def do_feed_list_and_verify(self, start, count=500):
        #reset all filters on the feed
        self.dict = {}
        self.post('/Feed/SetSettings.json?', self.dict)
        
        dictionary_get_items = self.get('/Feed/List.json?start='+str(start)+'&classId=&complete=true&count='+str(count))
        dictionary_verify_settingsforfeed = dictionary_get_items['data']['settingsforfeed']
        self.assertTrue('announcementtype' in dictionary_verify_settingsforfeed, 'announcementtype exists')
        self.assertTrue('sorttype' in dictionary_verify_settingsforfeed, 'sorttype exists')
        self.assertTrue('gradingperiodid' in dictionary_verify_settingsforfeed, 'announcementtype exists')
        
        announcementtype = str(dictionary_verify_settingsforfeed['announcementtype'])
        sorttype = str(dictionary_verify_settingsforfeed['sorttype'])
        gradingperiodid = str(dictionary_verify_settingsforfeed['gradingperiodid'])

        #verify that state of items is 'done'
        dictionary_verify_annoucementviewdatas_all = dictionary_get_items['data']['annoucementviewdatas']  
        for item in dictionary_verify_annoucementviewdatas_all:
            complete = item['complete']
            self.assertTrue(complete == True, "Verify item on true")

    def tearDown(self):
        get_all_unmarket_items = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(1000)) 
        for_item_id = get_all_unmarket_items['data']['annoucementviewdatas'] 
        for item in for_item_id:
            id = str(item['id'])
            type = str(item['type'])
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'true'})
    
if __name__ == '__main__':
    unittest.main()