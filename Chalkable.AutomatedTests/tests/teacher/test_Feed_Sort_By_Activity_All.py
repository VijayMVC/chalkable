from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        self.data2 = {'type':'3'}
        self.settings_data = {'announcementType':'3', 'sortType':'0'}
    
        self.post('/Feed/SetSettings.json?', self.settings_data)   
    
        print self.do_feed_list_and_verify(0)    
        self.do_feed_list_and_verify(10)
        self.do_feed_list_and_verify(20, 500) 

    def do_feed_list_and_verify(self, start, count=10):
        dictionary_get_items = self.get('/Feed/List.json?start='+str(start)+'&classId=&count='+str(count)) 
        dictionary_verify_settingsforfeed = dictionary_get_items['data']['settingsforfeed'] 
        self.assertTrue('announcementtype' in dictionary_verify_settingsforfeed, 'announcementtype exists') 
        self.assertTrue('sorttype' in dictionary_verify_settingsforfeed, 'sorttype exists') 
        announcementtype = str(dictionary_verify_settingsforfeed['announcementtype']) 
        sorttype = str(dictionary_verify_settingsforfeed['sorttype']) 
        self.assertDictContainsSubset(self.settings_data, {'announcementType':announcementtype, 'sortType':sorttype}, 'key/value pair exists')
    
        dictionary_verify_annoucementviewdatas_all = dictionary_get_items['data']['annoucementviewdatas']   
        for item in dictionary_verify_annoucementviewdatas_all: 
            type = str(item['type'])
            self.assertDictContainsSubset(self.data2, {'type':type}, 'key/value pair exists')
            
    def tearDown(self):
        get_all_unmarket_items = self.get('/Feed/List.json?start='+str(0)+'&classId=&complete=false&count='+str(1000)) 
        for_item_id = get_all_unmarket_items['data']['annoucementviewdatas'] 
        for item in for_item_id:
            id = str(item['id'])
            type = str(item['type'])
            #print id
            self.post('/Announcement/Complete', {'announcementId':id, 'announcementType':type, 'complete':'true'})
            
if __name__ == '__main__':
    unittest.main()