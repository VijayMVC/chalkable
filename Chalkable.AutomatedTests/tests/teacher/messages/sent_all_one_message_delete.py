from base_auth_test import *


class TestFeed(BaseAuthedTestCase):
    def test_feed(self):
        get_messages_inbox_all = self.get(
            '/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(750) + '&income=' + str(
                False) + '&role=' + '&classOnly=' + str(False))
        get_messages_inbox_all_data = get_messages_inbox_all['data']

        if len(get_messages_inbox_all_data) > 0:
            list_for_message_id = []
            for i in get_messages_inbox_all_data:
                for key, value in i['sentmessagedata'].iteritems():
                    if key == 'id':
                        list_for_message_id.append(value)

            list_converted_to_string = str(list_for_message_id)
            random_message = random.choice(list_for_message_id)

            # deleting one message
            data = {"ids": random_message,
                    "income": False}

            post_delete = self.postJSON('/PrivateMessage/Delete.json?', data)

            get_messages_inbox_all = self.get(
                '/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(750) + '&income=' + str(
                    False) + '&role=' + '&classOnly=' + str(False))
            get_messages_inbox_all_data = get_messages_inbox_all['data']

            list_for_message_id_second = []
            for i in get_messages_inbox_all_data:
                for key2, value2 in i['sentmessagedata'].iteritems():
                    if key2 == 'id':
                        list_for_message_id_second.append(value2)

            self.assertTrue(random_message not in list_for_message_id_second, "deleted message isn't in the list")


if __name__ == '__main__':
    unittest.main()
