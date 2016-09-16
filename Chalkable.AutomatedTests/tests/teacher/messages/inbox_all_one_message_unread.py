from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        get_messages_inbox_all = self.get('/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(750) + '&income=' + str(True) + '&role=' + '&classOnly=' + str(False))
        get_messages_inbox_all_data = get_messages_inbox_all['data']

        if len(get_messages_inbox_all_data) > 0:
            list_for_message_id = []

            for i in get_messages_inbox_all_data:
                for key, value in i['incomemessagedata'].iteritems():
                    if key == 'id':
                        list_for_message_id.append(value)

            list_converted_to_string = str(list_for_message_id)
            random_message = random.choice(list_for_message_id)

            # all messages in state read
            sliced_list = list_converted_to_string[1:-1]
            data = {"ids": sliced_list,
                    "read": True}

            post_unread = self.postJSON('/PrivateMessage/MarkAsRead.json?', data)

            # one message in state unread
            data = {"ids": random_message,
                    "read": False}

            post_unread = self.postJSON('/PrivateMessage/MarkAsRead.json?', data)


            get_messages_inbox_all = self.get('/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(750) + '&income=' + str(True) + '&role=' + '&classOnly=' + str(False))
            get_messages_inbox_all_data = get_messages_inbox_all['data']

            for info_about_one_message in get_messages_inbox_all_data:
                for data_inside_incomemessagedata in info_about_one_message['incomemessagedata'].iteritems():
                    if data_inside_incomemessagedata[1] == random_message:
                        for key, value in info_about_one_message['incomemessagedata'].iteritems():
                            if key == 'read':
                                self.assertTrue(value == False, 'one message is in state read')


if __name__ == '__main__':
    unittest.main()