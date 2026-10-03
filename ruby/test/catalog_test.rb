# frozen_string_literal: true

require_relative 'test_helper'

class CatalogTest < Minitest::Test
  def test_add_assigns_next_id_and_trims
    catalog = sample_catalog
    book = catalog.add(title: '  Emma ', author: ' Jane Austen', genre: 'Romance ', year: 1815)
    assert_equal 6, book.id
    assert_equal 'Emma', book.title
    assert_equal 6, catalog.count
  end

  def test_first_id_in_empty_catalog_is_one
    assert_equal 1, Catalog.new.add(title: 'A', author: 'B', genre: 'C', year: 2000).id
  end

  def test_add_rejects_invalid_data
    catalog = Catalog.new
    assert_raises(ArgumentError) { catalog.add(title: '', author: 'B', genre: 'C', year: 2000) }
    assert_raises(ArgumentError) { catalog.add(title: 'A', author: 'B', genre: 'C', year: 999) }
    assert_raises(ArgumentError) { catalog.add(title: 'A', author: 'B', genre: 'C', year: '2000') }
    assert_equal 0, catalog.count
  end

  def test_remove
    catalog = sample_catalog
    assert catalog.remove(2)
    refute catalog.remove(2)
    refute catalog.remove(999)
    assert_equal [1, 3, 4, 5], catalog.list_all.map(&:id)
  end

  def test_id_after_removing_highest_is_highest_plus_one
    catalog = sample_catalog
    catalog.remove(5)
    assert_equal 5, catalog.add(title: 'X', author: 'Y', genre: 'Z', year: 2000).id
  end

  def test_search_is_case_insensitive_partial_and_sorted_by_title
    results = sample_catalog.search_by_title('THE')
    assert_equal ['The Fellowship of the Ring', 'The Hobbit'], results.map(&:title)
    assert_equal ['Dune'], sample_catalog.search_by_title('  dUn ').map(&:title)
  end

  def test_search_by_author_and_genre
    assert_equal 2, sample_catalog.search_by_author('orwell').size
    assert_equal 2, sample_catalog.search_by_genre('FANTASY').size
    assert_empty sample_catalog.search_by_genre('romance')
  end

  def test_genre_report_groups_ignoring_case_and_sorts
    groups = sample_catalog.group_by_genre
    assert_equal ['Dystopian', 'Fantasy', 'Political Satire', 'Science Fiction'], groups.map(&:name)
    fantasy = groups.find { |group| group.name == 'Fantasy' }
    assert_equal [1, 5], fantasy.books.map(&:id) # sorted by year: 1937, 1954
  end

  def test_author_report_counts
    counts = sample_catalog.group_by_author.to_h { |group| [group.name, group.books.size] }
    assert_equal 2, counts['George Orwell']
    assert_equal 1, counts['Frank Herbert']
  end

  def test_catalog_is_enumerable
    assert_equal 5, sample_catalog.map(&:id).size
    assert_equal 1949, sample_catalog.find { |book| book.title == '1984' }.year
  end
end
