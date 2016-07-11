from base_auth_test import *

class TestFeed(BaseAuthedTestCase):
    def test_feed(self):

        get_messages_inbox_all = self.get(
            '/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(1000) + '&income=' + str(
                True) + '&role=' + '&classOnly=' + str(False))

        get_messages_inbox_all_data = get_messages_inbox_all['data']

        if len(get_messages_inbox_all_data) > 0:
            list_for_message_id = []
            for i in get_messages_inbox_all_data:
                for key, value in i['incomemessagedata'].iteritems():
                    if key == 'id':
                        list_for_message_id.append(value)

            list_converted_to_string = str(list_for_message_id)
            random_message = random.choice(list_for_message_id)

            sliced_list = list_converted_to_string[1:-1]

            # deleting all messages
            data = {"ids": sliced_list,
                    "income": True}

            post_delete = self.postJSON('/PrivateMessage/Delete.json?', data)


            teacher_id = self.id_of_current_teacher()

            data = {"body": "this is a body", "personId": teacher_id, "subject": "this is a subject"}

            # creating 15 messages
            for i in range(15):
                post_send = self.postJSON('/PrivateMessage/Send.json', data)


            get_messages_inbox_all = self.get(
                '/PrivateMessage/List.json?' + 'start=' + str(0) + '&count=' + str(1000) + '&income=' + str(
                    True) + '&role=' + '&classOnly=' + str(False))

            self.assertTrue (len(get_messages_inbox_all['data']) == 15, 'teacher has 11 messages')


if __name__ == '__main__':
    unittest.main()
