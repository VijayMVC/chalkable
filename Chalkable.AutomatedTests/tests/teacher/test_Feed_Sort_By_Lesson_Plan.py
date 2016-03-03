from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        self.data2 = {'type':'3'}
        self.settings_data = {'announcementType':'3', 'sortType':'0'}
    
        self.post('/Feed/SetSettings.json?', self.settings_data)   
    
        self.do_feed_list_and_verify(0)    
        self.do_feed_list_and_verify(10)
        self.do_feed_list_and_verify(20, 500) 

    def do_feed_list_and_verify(self, start, count=10):
        dictionary_get_items = self.get('/Feed/List.json?start='+str(start)+'&classId=&complete=false&count='+str(count)) #1
        dictionary_verify_settingsforfeed = dictionary_get_items['data']['settingsforfeed'] #2
        self.assertTrue('announcementtype' in dictionary_verify_settingsforfeed, 'announcementtype exists') #3
        self.assertTrue('sorttype' in dictionary_verify_settingsforfeed, 'sorttype exists') #4
        announcementtype = str(dictionary_verify_settingsforfeed['announcementtype']) #5
        sorttype = str(dictionary_verify_settingsforfeed['sorttype']) #6
        self.assertDictContainsSubset(self.settings_data, {'announcementType':announcementtype, 'sortType':sorttype}, 'key/value pair exists') #7
    
        dictionary_verify_annoucementviewdatas_all = dictionary_get_items['data']['annoucementviewdatas'] #8    
        for item in dictionary_verify_annoucementviewdatas_all: #9
            type = str(item['type'])
            self.assertDictContainsSubset(self.data2, {'type':type}, 'key/value pair exists')
    
        return dictionary_get_items
        
if __name__ == '__main__':
    unittest.main()