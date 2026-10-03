# frozen_string_literal: true

require_relative 'test_helper'

class StorageTest < Minitest::Test
  def with_temp_file(contents = nil)
    Dir.mktmpdir do |dir|
      path = File.join(dir, 'books.json')
      File.write(path, contents) unless contents.nil?
      yield path
    end
  end

  def test_missing_file
    with_temp_file do |path|
      result = Storage.load(path)
      assert_equal :not_found, result.status
      assert_empty result.books
    end
  end

  def test_save_then_load_round_trip
    with_temp_file do |path|
      assert Storage.save(path, sample_catalog)
      result = Storage.load(path)
      assert_equal :loaded, result.status
      assert_equal sample_catalog.map(&:to_s), result.books.map(&:to_s)
    end
  end

  def test_save_creates_missing_folder
    Dir.mktmpdir do |dir|
      path = File.join(dir, 'new_folder', 'books.json')
      assert Storage.save(path, [])
      assert_equal "[]\n", File.read(path)
    end
  end

  def test_corrupt_files_are_detected
    bad_files = [
      'not json', '', '{"id": 1}',
      '[{"id":1,"title":"A","author":"B","genre":"C"}]',
      '[{"id":1,"title":"A","author":"B","genre":"C","year":"1990"}]',
      '[{"id":1,"title":"A","author":"B","genre":"C","year":3000}]',
      '[{"id":1,"title":"A","author":"B","genre":"C","year":1990},{"id":1,"title":"D","author":"E","genre":"F","year":1991}]'
    ]
    bad_files.each do |contents|
      with_temp_file(contents) do |path|
        assert_equal :corrupt, Storage.load(path).status, "expected corrupt: #{contents.inspect}"
      end
    end
  end
end
